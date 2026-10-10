using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Json;

namespace WinHubX.Impostazioni;

internal enum ElevatedProcessKind
{
    Dism,
    Registry,
    SystemUtility
}

internal static class ElevatedProcessCommandValidator
{
    private static readonly HashSet<string> AllowedRegistryHives = new(StringComparer.OrdinalIgnoreCase)
    {
        "TK_COMPONENTS", "TK_DEFAULT", "TK_NTUSER", "TK_SOFTWARE", "TK_SYSTEM", "TK_BOOT_SYSTEM"
    };

    private static readonly HashSet<string> AllowedDismOperations = new(StringComparer.OrdinalIgnoreCase)
    {
        "/export-image", "/mount-image", "/unmount-image", "/online"
    };
    private static readonly string[] RemovablePackagePrefixes =
    [
        "Microsoft-Windows-InternetExplorer-Optional-Package",
        "Microsoft-Windows-Kernel-LA57-FoD",
        "Microsoft-Windows-LanguageFeatures-Handwriting",
        "Microsoft-Windows-LanguageFeatures-OCR",
        "Microsoft-Windows-LanguageFeatures-Speech",
        "Microsoft-Windows-LanguageFeatures-TextToSpeech",
        "Microsoft-Windows-MediaPlayer-Package",
        "Microsoft-Windows-TabletPCMath-Package",
        "Microsoft-Windows-Wallpaper-Content-Extended-FoD"
    ];

    internal static void Validate(ElevatedProcessKind kind, IReadOnlyList<string> arguments, string workspaceRoot)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        if (arguments.Count is 0 or > 32)
            throw new ArgumentOutOfRangeException(nameof(arguments), "Numero di argomenti del processo privilegiato non valido.");
        if (arguments.Any(static argument => argument is null || argument.Length > 32_000 || argument.Any(char.IsControl)))
            throw new ArgumentException("Argomento del processo privilegiato non valido.", nameof(arguments));
        if (arguments.Sum(static argument => (long)argument.Length) > 64_000)
            throw new ArgumentException("Gli argomenti del processo privilegiato superano il limite consentito.", nameof(arguments));

        switch (kind)
        {
            case ElevatedProcessKind.Dism:
                ValidateDism(arguments, workspaceRoot);
                break;
            case ElevatedProcessKind.Registry:
                ValidateRegistry(arguments, workspaceRoot);
                break;
            case ElevatedProcessKind.SystemUtility:
                ValidateSystemUtility(arguments, workspaceRoot);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind));
        }
    }

    private static void ValidateDism(IReadOnlyList<string> arguments, string workspaceRoot)
    {
        string root = EnsureTrailingSeparator(Path.GetFullPath(workspaceRoot));
        bool offlineDriverOperation = arguments[0].StartsWith("/Image:", StringComparison.OrdinalIgnoreCase);
        if (!offlineDriverOperation && !AllowedDismOperations.Contains(arguments[0]))
            throw new ArgumentException("Operazione DISM non consentita.", nameof(arguments));

        if (offlineDriverOperation)
        {
            string imageMountPath = Path.GetFullPath(arguments[0]["/Image:".Length..]);
            string installMount = Path.GetFullPath(Path.Join(root, "Mount", "mount"));
            string bootMount = Path.GetFullPath(Path.Join(root, "Mount", "boot"));
            string[] driverArguments = arguments.Skip(1).ToArray();
            if (driverArguments.Length == 3
                && driverArguments.Contains("/Add-Driver", StringComparer.OrdinalIgnoreCase)
                && driverArguments.Contains("/Recurse", StringComparer.OrdinalIgnoreCase))
            {
                string[] driverPaths = driverArguments
                    .Where(static argument => argument.StartsWith("/Driver:", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if ((!imageMountPath.Equals(installMount, StringComparison.OrdinalIgnoreCase)
                        && !imageMountPath.Equals(bootMount, StringComparison.OrdinalIgnoreCase))
                    || driverPaths.Length != 1)
                    throw new ArgumentException("Sono consentite solo integrazioni driver ricorsive nelle immagini WinHubX montate.", nameof(arguments));

                string driverPath = driverPaths[0]["/Driver:".Length..];
                if (!Path.IsPathFullyQualified(driverPath) || driverPath.StartsWith("\\\\", StringComparison.Ordinal))
                    throw new ArgumentException("La cartella driver deve essere un percorso locale assoluto.", nameof(arguments));
                _ = Path.GetFullPath(driverPath);
                return;
            }

            if (!imageMountPath.Equals(installMount, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Le operazioni sui pacchetti sono consentite solo sull'immagine Windows di installazione.", nameof(arguments));

            if (driverArguments.Length == 3
                && driverArguments[0].Equals("/English", StringComparison.OrdinalIgnoreCase)
                && driverArguments[1].Equals("/Get-Packages", StringComparison.OrdinalIgnoreCase)
                && driverArguments[2].Equals("/Format:List", StringComparison.OrdinalIgnoreCase))
                return;

            if (driverArguments.Length == 4
                && driverArguments[0].Equals("/English", StringComparison.OrdinalIgnoreCase)
                && driverArguments[1].Equals("/Remove-Package", StringComparison.OrdinalIgnoreCase)
                && driverArguments[3].Equals("/NoRestart", StringComparison.OrdinalIgnoreCase)
                && driverArguments[2].StartsWith("/PackageName:", StringComparison.OrdinalIgnoreCase)
                && IsAllowedPackageIdentity(driverArguments[2]["/PackageName:".Length..]))
                return;

            throw new ArgumentException("Operazione DISM offline non consentita.", nameof(arguments));
        }
        else if (arguments[0].Equals("/Online", StringComparison.OrdinalIgnoreCase))
        {
            bool exportDriver = arguments.Count == 3
                && arguments[1].Equals("/Export-Driver", StringComparison.OrdinalIgnoreCase)
                && arguments[2].StartsWith("/Destination:", StringComparison.OrdinalIgnoreCase)
                && Path.GetFullPath(arguments[2]["/Destination:".Length..])
                    .StartsWith(root + "DriverExport" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            bool safeRepair = arguments.Count == 3
                && arguments[1].Equals("/Cleanup-Image", StringComparison.OrdinalIgnoreCase)
                && (arguments[2].Equals("/CheckHealth", StringComparison.OrdinalIgnoreCase)
                    || arguments[2].Equals("/ScanHealth", StringComparison.OrdinalIgnoreCase)
                    || arguments[2].Equals("/RestoreHealth", StringComparison.OrdinalIgnoreCase)
                    || arguments[2].Equals("/StartComponentCleanup", StringComparison.OrdinalIgnoreCase));
            if (!exportDriver && !safeRepair)
            {
                throw new ArgumentException("Operazione DISM online non consentita.", nameof(arguments));
            }
            return;
        }

        foreach (string argument in arguments.Skip(1))
        {
            if (argument.StartsWith('/') && !argument.StartsWith("/ImageFile:", StringComparison.OrdinalIgnoreCase)
                && !argument.StartsWith("/SourceImageFile:", StringComparison.OrdinalIgnoreCase)
                && !argument.StartsWith("/DestinationImageFile:", StringComparison.OrdinalIgnoreCase)
                && !argument.StartsWith("/MountDir:", StringComparison.OrdinalIgnoreCase)
                && !argument.StartsWith("/SourceIndex:", StringComparison.OrdinalIgnoreCase)
                && !argument.StartsWith("/Index:", StringComparison.OrdinalIgnoreCase)
                && !argument.Equals("/Compress:max", StringComparison.OrdinalIgnoreCase)
                && !argument.Equals("/CheckIntegrity", StringComparison.OrdinalIgnoreCase)
                && !argument.Equals("/commit", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Parametro DISM non consentito.", nameof(arguments));
            }
        }

        foreach (string argument in arguments.Skip(1))
        {
            string? filePath = GetNamedPathArgument(argument, "/ImageFile:", "/MountDir:", "/DestinationImageFile:");
            if (filePath is not null && !Path.GetFullPath(filePath).StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("DISM può operare solo sui file e mount WinHubX della sessione corrente.", nameof(arguments));

            if (argument.StartsWith("/SourceImageFile:", StringComparison.OrdinalIgnoreCase)
                && !Path.GetFullPath(argument["/SourceImageFile:".Length..]).StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("L'immagine sorgente DISM deve appartenere alla sessione ISO corrente.", nameof(arguments));
            }
        }
    }

    private static void ValidateSystemUtility(IReadOnlyList<string> arguments, string workspaceRoot)
    {
        if (arguments[0].Equals("sfc", StringComparison.OrdinalIgnoreCase)
            && arguments.Count == 2 && arguments[1].Equals("/scannow", StringComparison.OrdinalIgnoreCase))
            return;

        if (arguments[0].Equals("chkdsk", StringComparison.OrdinalIgnoreCase)
            && arguments.Count == 3 && arguments[1].Length == 2 && char.IsAsciiLetter(arguments[1][0])
            && arguments[1][1] == ':' && arguments[2].Equals("/scan", StringComparison.OrdinalIgnoreCase))
            return;

        if (arguments[0].Equals("regsvr32", StringComparison.OrdinalIgnoreCase)
            && arguments.Count == 3 && arguments[1].Equals("/s", StringComparison.OrdinalIgnoreCase)
            && AllowedSystemDlls.Contains(arguments[2], StringComparer.OrdinalIgnoreCase))
            return;

        string expectedBackupPath = Path.GetFullPath(Path.Join(workspaceRoot, "Repair", "RegistryBackup_HKLM.reg"));
        if (arguments[0].Equals("export-hklm", StringComparison.OrdinalIgnoreCase)
            && arguments.Count == 2
            && Path.GetFullPath(arguments[1]).Equals(expectedBackupPath, StringComparison.OrdinalIgnoreCase))
            return;

        throw new ArgumentException("Comando di ripristino elevato non consentito.", nameof(arguments));
    }

    private static readonly string[] AllowedSystemDlls =
    ["atl.dll", "jscript.dll", "msxml3.dll", "shell32.dll", "shdocvw.dll", "urlmon.dll", "vbscript.dll", "wintrust.dll"];

    internal static IReadOnlyList<string> ParseInstalledPackageIdentities(string dismOutput)
    {
        ArgumentNullException.ThrowIfNull(dismOutput);
        List<string> installedPackages = [];
        string? identity = null;
        bool installed = false;

        void AddCurrentPackage()
        {
            if (identity is not null && installed && IsAllowedPackageIdentity(identity))
                installedPackages.Add(identity);
        }

        foreach (string line in dismOutput.Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(static rawLine => rawLine.Trim()))
        {
            if (line.StartsWith("Package Identity", StringComparison.OrdinalIgnoreCase))
            {
                AddCurrentPackage();
                int separator = line.IndexOf(':');
                identity = separator >= 0 ? line[(separator + 1)..].Trim() : null;
                installed = false;
            }
            else if (identity is not null && line.StartsWith("State", StringComparison.OrdinalIgnoreCase))
            {
                int separator = line.IndexOf(':');
                installed = separator >= 0 && line[(separator + 1)..].Trim()
                    .Equals("Installed", StringComparison.OrdinalIgnoreCase);
            }
        }

        AddCurrentPackage();
        return installedPackages;
    }

    private static bool IsAllowedPackageIdentity(string identity)
        => identity.Length is > 0 and <= 512
            && RemovablePackagePrefixes.Any(prefix => identity.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            && identity.All(static character => char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.' or '~' or '+');

    private static void ValidateRegistry(IReadOnlyList<string> arguments, string workspaceRoot)
    {
        string operation = arguments[0];
        if (operation.Equals("load", StringComparison.OrdinalIgnoreCase))
        {
            if (arguments.Count != 3)
                throw new ArgumentException("Comando reg load non valido.", nameof(arguments));
            _ = GetAllowedHive(arguments[1]);
            string imagePath = Path.GetFullPath(arguments[2]);
            string root = EnsureTrailingSeparator(Path.GetFullPath(workspaceRoot));
            string[] allowedRoots = [root + @"Mount\mount\", root + @"Mount\boot\"];
            if (!allowedRoots.Any(allowedRoot => imagePath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("L'hive da caricare deve appartenere a una directory di mount WinHubX.", nameof(arguments));
            string[] allowedHiveFiles =
            [
                @"\Windows\System32\config\COMPONENTS",
                @"\Windows\System32\config\default",
                @"\Windows\System32\config\SOFTWARE",
                @"\Windows\System32\config\SYSTEM",
                @"\Users\Default\ntuser.dat"
            ];
            if (!allowedHiveFiles.Any(suffix => imagePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("File hive non consentito.", nameof(arguments));
            return;
        }

        if (operation.Equals("unload", StringComparison.OrdinalIgnoreCase))
        {
            if (arguments.Count != 2)
                throw new ArgumentException("Comando reg unload non valido.", nameof(arguments));
            _ = GetAllowedHive(arguments[1]);
            return;
        }

        if (operation is not ("add" or "delete") || arguments.Count < 3)
            throw new ArgumentException("Operazione reg non consentita.", nameof(arguments));

        _ = GetAllowedHive(arguments[1]);
        string[] keySegments = arguments[1].Split('\\');
        if (keySegments.Any(static segment => segment is "" or "." or ".."))
            throw new ArgumentException("Percorso Registro non valido.", nameof(arguments));
        if (arguments.Skip(2).Any(static argument => argument.StartsWith("/reg:", StringComparison.OrdinalIgnoreCase)
            || argument.StartsWith("/reg ", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("La selezione arbitraria della vista Registro non è consentita.", nameof(arguments));
        }
    }

    private static string GetAllowedHive(string target)
    {
        const string prefix = "HKLM\\";
        if (!target.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Il comando Registro deve agire esclusivamente sugli hive temporanei HKLM di WinHubX.", nameof(target));

        string hive = target[prefix.Length..].Split('\\', 2)[0];
        if (!AllowedRegistryHives.Contains(hive))
            throw new ArgumentException("Hive Registro non autorizzato.", nameof(target));
        return hive;
    }

    private static string? GetNamedPathArgument(string argument, params string[] prefixes)
    {
        foreach (string prefix in prefixes)
        {
            if (argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return argument[prefix.Length..];
        }
        return null;
    }

    private static string EnsureTrailingSeparator(string path)
        => Path.EndsInDirectorySeparator(path) ? path : path + Path.DirectorySeparatorChar;
}

[SupportedOSPlatform("windows")]
internal sealed class ElevatedProcessBrokerClient : IAsyncDisposable
{
    private const string BrokerArgument = "--elevated-process-broker";
    private readonly NamedPipeServerStream _pipe;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly Process _brokerProcess;
    private readonly SemaphoreSlim _requestGate = new(1, 1);
    private int _disposed;

    private ElevatedProcessBrokerClient(NamedPipeServerStream pipe, StreamReader reader, StreamWriter writer, Process brokerProcess, string workspaceRoot)
    {
        _pipe = pipe;
        _reader = reader;
        _writer = writer;
        _brokerProcess = brokerProcess;
        WorkspaceRoot = workspaceRoot;
    }

    internal string WorkspaceRoot { get; }

    internal static async Task<ElevatedProcessBrokerClient> StartAsync(CancellationToken cancellationToken)
    {
        string pipeName = $"WinHubX-{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        using WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
        SecurityIdentifier userSid = currentIdentity.User
            ?? throw new UnauthorizedAccessException("Impossibile determinare l'identità Windows corrente.");
        string workspaceRoot = GetWorkspaceRootWithMostFreeSpace();

        PipeSecurity pipeSecurity = new();
        pipeSecurity.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        pipeSecurity.SetOwner(userSid);
        AddPipeAccess(pipeSecurity, userSid, PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance);
        AddPipeAccess(pipeSecurity, new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance);
        AddPipeAccess(pipeSecurity, new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
            PipeAccessRights.FullControl);
        using ResourceOwner<NamedPipeServerStream> pipeOwner = new(NamedPipeServerStreamAcl.Create(
            pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity));
        using ResourceOwner<Process> brokerProcessOwner = new();
        StreamReader? reader = null;
        StreamWriter? writer = null;

        try
        {
            string executablePath = Path.Join(AppContext.BaseDirectory, "WinHubX.exe");
            if (!File.Exists(executablePath))
                throw new FileNotFoundException("Eseguibile WinHubX necessario per il broker UAC non trovato.", executablePath);

            ProcessStartInfo startInfo = new(executablePath)
            {
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            startInfo.ArgumentList.Add(BrokerArgument);
            startInfo.ArgumentList.Add(pipeName);
            startInfo.ArgumentList.Add(token);
            Process brokerProcess = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il broker privilegiato.");
            brokerProcessOwner.Set(brokerProcess);

            NamedPipeServerStream pipe = pipeOwner.Value;
            await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
            reader = new StreamReader(pipe, leaveOpen: true);
            writer = new StreamWriter(pipe, leaveOpen: true) { AutoFlush = true };
            await writer.WriteLineAsync(JsonSerializer.Serialize(new BrokerHandshake(token, workspaceRoot, userSid.Value)).AsMemory(), cancellationToken)
                .ConfigureAwait(false);
            string? acknowledgement = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (acknowledgement != "READY")
                throw new UnauthorizedAccessException("Il broker privilegiato non ha autenticato il canale IPC.");

            ElevatedProcessBrokerClient client = new(
                pipeOwner.Transfer(), reader, writer, brokerProcessOwner.Transfer(), workspaceRoot);
            return client;
        }
        catch
        {
            try
            {
                reader?.Dispose();
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                Debug.WriteLine($"Chiusura reader del broker fallita: {ex.Message}");
            }

            try
            {
                writer?.Dispose();
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                Debug.WriteLine($"Chiusura writer del broker fallita: {ex.Message}");
            }

            Process? brokerProcess = brokerProcessOwner.ValueOrDefault;
            if (brokerProcess is not null)
            {
                try
                {
                    if (!brokerProcess.HasExited)
                        brokerProcess.Kill(entireProcessTree: true);
                }
                catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
                {
                    Debug.WriteLine($"Impossibile terminare il broker dopo un errore di avvio: {ex.Message}");
                }
            }
            throw;
        }
    }

    internal Task<int> RunDismAsync(IReadOnlyList<string> arguments, Action<string, bool>? onOutput, CancellationToken cancellationToken)
        => RunAsync(ElevatedProcessKind.Dism, arguments, onOutput, cancellationToken);

    internal Task<int> RunRegistryAsync(IReadOnlyList<string> arguments, Action<string, bool>? onOutput, CancellationToken cancellationToken)
        => RunAsync(ElevatedProcessKind.Registry, arguments, onOutput, cancellationToken);

    internal Task<int> RunSystemUtilityAsync(IReadOnlyList<string> arguments, Action<string, bool>? onOutput, CancellationToken cancellationToken)
        => RunAsync(ElevatedProcessKind.SystemUtility, arguments, onOutput, cancellationToken);

    private async Task<int> RunAsync(
        ElevatedProcessKind kind,
        IReadOnlyList<string> arguments,
        Action<string, bool>? onOutput,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        ElevatedProcessCommandValidator.Validate(kind, arguments, WorkspaceRoot);
        await _requestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _writer.WriteLineAsync(JsonSerializer.Serialize(new BrokerRequest(kind, arguments.ToArray())).AsMemory(), cancellationToken)
                .ConfigureAwait(false);
            while (true)
            {
                string? line = await _reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (line is null)
                    throw new EndOfStreamException("Il broker privilegiato ha chiuso il canale durante l'operazione.");
                BrokerResponse response = JsonSerializer.Deserialize<BrokerResponse>(line)
                    ?? throw new InvalidDataException("Risposta non valida dal broker privilegiato.");
                if (response.Type == "output")
                    onOutput?.Invoke(response.Text ?? string.Empty, response.IsError);
                else if (response.Type == "exit" && response.ExitCode.HasValue)
                    return response.ExitCode.Value;
                else
                    throw new InvalidDataException("Tipo di risposta non riconosciuto dal broker privilegiato.");
            }
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!_brokerProcess.HasExited)
                    _brokerProcess.Kill(entireProcessTree: true);
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                Debug.WriteLine($"Impossibile arrestare il broker privilegiato: {ex.Message}");
            }
            throw;
        }
        finally
        {
            _requestGate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        try
        {
            if (!_brokerProcess.HasExited)
            {
                await _writer.WriteLineAsync(JsonSerializer.Serialize(new BrokerRequest(null, []))).ConfigureAwait(false);
                using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));
                await _brokerProcess.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or IOException or InvalidOperationException)
        {
            try
            {
                if (!_brokerProcess.HasExited)
                    _brokerProcess.Kill(entireProcessTree: true);
            }
            catch (Exception killException) when (killException is InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                Debug.WriteLine($"Impossibile terminare il broker privilegiato: {killException.Message}");
            }
        }
        await DisposeResourcesAsync().ConfigureAwait(false);
    }

    private async ValueTask DisposeResourcesAsync()
    {
        try
        {
            _reader.Dispose();
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            Debug.WriteLine($"Chiusura reader del broker fallita: {ex.Message}");
        }

        try
        {
            await _writer.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            Debug.WriteLine($"Chiusura writer del broker fallita: {ex.Message}");
        }

        try
        {
            await _pipe.DisposeAsync().ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Chiusura pipe del broker fallita: {ex.Message}");
        }

        _brokerProcess.Dispose();
        _requestGate.Dispose();
    }

    internal static async Task<int> RunHostAsync(string pipeName, string expectedToken)
    {
        if (!OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(pipeName)
            || pipeName.Length > 80 || pipeName.Any(static character => !char.IsAsciiLetterOrDigit(character) && character != '-'))
            return 87;

        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);
        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            return 5;

        using NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        await pipe.ConnectAsync(10_000).ConfigureAwait(false);
        using StreamReader reader = new(pipe, leaveOpen: true);
        using StreamWriter writer = new(pipe, leaveOpen: true) { AutoFlush = true };
        string? handshakeLine = await reader.ReadLineAsync().ConfigureAwait(false);
        BrokerHandshake? handshake = handshakeLine is null ? null : JsonSerializer.Deserialize<BrokerHandshake>(handshakeLine);
        if (handshake?.Token is null || !TokensEqual(handshake.Token, expectedToken)
            || string.IsNullOrWhiteSpace(handshake.WorkspaceRoot) || string.IsNullOrWhiteSpace(handshake.UserSid))
            return 5;

        string workspaceRoot = PrepareWorkspace(handshake.WorkspaceRoot, handshake.UserSid);

        await writer.WriteLineAsync("READY").ConfigureAwait(false);
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } requestLine)
        {
            BrokerRequest? request = JsonSerializer.Deserialize<BrokerRequest>(requestLine);
            if (request is null)
                return 87;
            if (request.Kind is null)
                return 0;

            IReadOnlyList<string> arguments = request.Arguments ?? [];
            ElevatedProcessKind kind = request.Kind.Value;
            ElevatedProcessCommandValidator.Validate(kind, arguments, workspaceRoot);
            int exitCode = await RunHostProcessAsync(kind, arguments, writer).ConfigureAwait(false);
            await writer.WriteLineAsync(JsonSerializer.Serialize(new BrokerResponse("exit", ExitCode: exitCode))).ConfigureAwait(false);
        }

        return 0;
    }

    private static async Task<int> RunHostProcessAsync(ElevatedProcessKind kind, IReadOnlyList<string> arguments, StreamWriter writer)
    {
        string executableName = kind switch
        {
            ElevatedProcessKind.Dism => "dism.exe",
            ElevatedProcessKind.Registry => "reg.exe",
            ElevatedProcessKind.SystemUtility => arguments[0].ToLowerInvariant() switch
            {
                "sfc" => "sfc.exe",
                "chkdsk" => "chkdsk.exe",
                "regsvr32" => "regsvr32.exe",
                "export-hklm" => "reg.exe",
                _ => throw new InvalidOperationException("Utility privilegiata non riconosciuta dopo la validazione.")
            },
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        string executablePath = Path.Join(Environment.SystemDirectory, executableName);
        IEnumerable<string> processArguments = kind == ElevatedProcessKind.SystemUtility
            ? GetSystemUtilityArguments(arguments)
            : arguments;
        ProcessStartInfo startInfo = new(executablePath)
        {
            WorkingDirectory = Environment.SystemDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (string argument in processArguments)
            startInfo.ArgumentList.Add(argument);

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Impossibile avviare {Path.GetFileName(executablePath)}.");
        using SemaphoreSlim writeGate = new(1, 1);
        Task stdoutTask = ForwardLinesAsync(process.StandardOutput, false, writer, writeGate);
        Task stderrTask = ForwardLinesAsync(process.StandardError, true, writer, writeGate);
        await process.WaitForExitAsync().ConfigureAwait(false);
        await Task.WhenAll(stdoutTask, stderrTask).ConfigureAwait(false);
        return process.ExitCode;
    }

    private static IReadOnlyList<string> GetSystemUtilityArguments(IReadOnlyList<string> arguments)
        => arguments[0].ToLowerInvariant() switch
        {
            "sfc" or "chkdsk" or "regsvr32" => arguments.Skip(1).ToArray(),
            "export-hklm" => ["export", "HKLM\\SOFTWARE", arguments[1], "/y"],
            _ => throw new InvalidOperationException("Utility privilegiata non riconosciuta dopo la validazione.")
        };

    private static async Task ForwardLinesAsync(StreamReader source, bool isError, StreamWriter writer, SemaphoreSlim writeGate)
    {
        while (await source.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            await writeGate.WaitAsync().ConfigureAwait(false);
            try
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(new BrokerResponse("output", line, IsError: isError))).ConfigureAwait(false);
            }
            finally
            {
                writeGate.Release();
            }
        }
    }

    private static bool TokensEqual(string received, string expected)
    {
        try
        {
            byte[] receivedBytes = Convert.FromBase64String(received);
            byte[] expectedBytes = Convert.FromBase64String(expected);
            return receivedBytes.Length == expectedBytes.Length
                && CryptographicOperations.FixedTimeEquals(receivedBytes, expectedBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static void AddPipeAccess(PipeSecurity security, SecurityIdentifier identity, PipeAccessRights rights)
    {
        security.AddAccessRule(new PipeAccessRule(identity, rights, AccessControlType.Allow));
    }

    private static void AddDirectoryAccess(DirectorySecurity security, SecurityIdentifier identity, FileSystemRights rights)
    {
        security.AddAccessRule(new FileSystemAccessRule(identity, rights,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None, AccessControlType.Allow));
    }

    private static string GetWorkspaceRootWithMostFreeSpace()
    {
        DriveInfo[] readyFixedDrives = DriveInfo.GetDrives()
            .Where(static drive => drive.DriveType == DriveType.Fixed)
            .Where(static drive =>
            {
                try { return drive.IsReady; }
                catch (IOException) { return false; }
                catch (UnauthorizedAccessException) { return false; }
            })
            .OrderByDescending(static drive =>
            {
                try { return drive.AvailableFreeSpace; }
                catch (IOException) { return 0L; }
                catch (UnauthorizedAccessException) { return 0L; }
            })
            .ToArray();
        if (readyFixedDrives.Length == 0)
            throw new IOException("Non è disponibile alcun disco fisso locale per il workspace ISO.");

        string root = readyFixedDrives[0].RootDirectory.FullName;
        return Path.GetFullPath(Path.Join(root, "WinHubX", "IsoSessions", Guid.NewGuid().ToString("N")));
    }

    private static string PrepareWorkspace(string workspaceRoot, string userSidValue)
    {
        string fullPath = Path.GetFullPath(workspaceRoot);
        string driveRoot = Path.GetPathRoot(fullPath)
            ?? throw new ArgumentException("Root del workspace non valido.", nameof(workspaceRoot));
        DriveInfo drive = new(driveRoot);
        if (!drive.IsReady || drive.DriveType != DriveType.Fixed)
            throw new IOException("Il workspace deve risiedere su un disco fisso locale disponibile.");

        string relativePath = Path.GetRelativePath(driveRoot, fullPath);
        string[] segments = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (segments.Length != 3
            || !segments[0].Equals("WinHubX", StringComparison.OrdinalIgnoreCase)
            || !segments[1].Equals("IsoSessions", StringComparison.OrdinalIgnoreCase)
            || !Guid.TryParseExact(segments[2], "N", out _))
        {
            throw new ArgumentException("Il workspace non appartiene al percorso di sessione WinHubX atteso.", nameof(workspaceRoot));
        }

        SecurityIdentifier userSid = new(userSidValue);
        string parentPath = Path.Join(driveRoot, segments[0], segments[1]);
        string firstParent = Path.Join(driveRoot, segments[0]);
        Directory.CreateDirectory(firstParent);
        EnsureNotReparsePoint(firstParent);
        Directory.CreateDirectory(parentPath);
        EnsureNotReparsePoint(parentPath);
        if (Directory.Exists(fullPath) || File.Exists(fullPath))
            throw new IOException("Il workspace casuale esiste già; per sicurezza non verrà riutilizzato.");

        DirectorySecurity security = new();
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.SetOwner(userSid);
        AddDirectoryAccess(security, userSid, FileSystemRights.FullControl);
        AddDirectoryAccess(security, new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl);
        AddDirectoryAccess(security, new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl);
        DirectoryInfo workspace = Directory.CreateDirectory(fullPath);
        workspace.SetAccessControl(security);
        EnsureNotReparsePoint(fullPath);
        return fullPath;
    }

    private static void EnsureNotReparsePoint(string path)
    {
        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.ReparsePoint) != 0)
            throw new IOException($"Il percorso workspace contiene un reparse point non consentito: {path}");
    }

    private sealed class ResourceOwner<T>(T? resource = null) : IDisposable where T : class, IDisposable
    {
        private T? _resource = resource;

        internal T Value => _resource ?? throw new InvalidOperationException("La risorsa non è stata inizializzata.");

        internal T? ValueOrDefault => _resource;

        internal void Set(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (Interlocked.CompareExchange(ref _resource, value, null) is not null)
                throw new InvalidOperationException("La risorsa è già stata inizializzata.");
        }

        internal T Transfer()
        {
            return Interlocked.Exchange(ref _resource, null)
                ?? throw new InvalidOperationException("La risorsa è già stata trasferita o non è inizializzata.");
        }

        public void Dispose() => Interlocked.Exchange(ref _resource, null)?.Dispose();
    }

    private sealed record BrokerHandshake(string Token, string WorkspaceRoot, string UserSid);
    private sealed record BrokerRequest(ElevatedProcessKind? Kind, string[] Arguments);
    private sealed record BrokerResponse(string Type, string? Text = null, bool IsError = false, int? ExitCode = null);
}
