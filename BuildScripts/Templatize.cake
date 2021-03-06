/// <summary>
/// This class contains contants that are used
/// to Template files.
/// </summary>
public class TemplateConstants
{
    // ---------------- Fields ----------------

    private readonly string chaskisCoreProjectContents;

    // ---------------- Constructor ----------------

    public TemplateConstants( ICakeContext context, ImportantPaths paths, string runTime )
    {
        this.CakeContext = context;
        this.ImportantPaths = paths;

        this.chaskisCoreProjectContents = this.CakeContext.FileReadText( this.ImportantPaths.ChaskisCoreVersionFile );

        this.FullName = "Chaskis IRC Bot";
        this.ChaskisVersion = this.GetChaskisVersion();
        this.ChaskisCoreVersion = this.GetChaskisCoreVersion();
        this.License = this.GetLicense();
        this.RegressionTestPluginVersion = this.GetRegressionTestPluginVersion();
        this.RegressionTestPluginName = this.GetRegressionTestPluginName();
        this.DefaultPluginName = this.GetDefaultPluginName();

        string tags = this.GetTags();
        this.CoreTags = tags + " core";
        this.CliTags = tags + " service full installer admin";
        this.Author = this.GetAuthor();
        this.AuthorEmail = "seth@shendrick.net";
        this.ProjectUrl = this.GetProjectUrl();
        this.LicenseUrl = this.GetLicenseUrl();
        this.WikiUrl = "https://github.com/xforever1313/Chaskis/wiki";
        this.IssueTrackerUrl = "https://github.com/xforever1313/Chaskis/issues";
        this.CopyRight = this.GetCopyRight();
        this.Description = this.GetDescription();
        this.ReleaseNotes = this.GetReleaseNotes();
        this.Summary = "A generic framework written in C# for making IRC Bots.";
        this.IconUrl = this.GetIconUrl();
        this.RunTime = runTime;
        this.DebChecksum = this.GetDebChecksum();
        this.MsiChecksum = this.GetMsiChecksum();
    }

    // ---------------- Properties ----------------

    public ICakeContext CakeContext { get; private set; }

    public ImportantPaths ImportantPaths { get; private set; }

    public string FullName { get; private set; }

    public string ChaskisVersion { get; private set; }

    public string ChaskisCoreVersion { get; private set; }

    public string License { get; private set; }

    public string RegressionTestPluginVersion { get; private set; }

    public string RegressionTestPluginName { get; private set; }

    public string DefaultPluginName { get; private set; }

    public string CoreTags { get; private set; }

    public string CliTags { get; private set; }

    public string Author { get; private set; }

    public string AuthorEmail { get; private set; }

    public string ProjectUrl { get; private set; }

    public string LicenseUrl { get; private set; }

    public string WikiUrl { get; private set; }

    public string IssueTrackerUrl { get; private set; }

    public string CopyRight { get; private set; }

    public string Description { get; private set; }

    public string ReleaseNotes { get; private set; }

    public string Summary { get; private set; }

    public string IconUrl { get; private set; }

    public string RunTime { get; private set; }

    public string DebChecksum { get; private set; }

    public string MsiChecksum { get; private set; }

    // ---------------- Functions ----------------

    private string GetChaskisVersion()
    {
        string fileContents = this.CakeContext.FileReadText( this.ImportantPaths.ChaskisVersionFile );
        Match match = Regex.Match(
            fileContents,
            @"public\s+const\s+string\s+VersionStr\s+=\s+""(?<version>\d+\.\d+\.\d+)"";"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Chaskis Version Regex" );
        }

        return match.Groups["version"].Value;
    }

    private string GetChaskisCoreVersion()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<Version\>(?<version>\d+\.\d+\.\d+)\</Version\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Chaskis Core Version Regex" );
        }

        return match.Groups["version"].Value;
    }

    private string GetLicense()
    {
        return this.CakeContext.FileReadText( this.ImportantPaths.LicenseFile );
    }

    private string GetRegressionTestPluginVersion()
    {
        string fileContents = this.CakeContext.FileReadText( this.ImportantPaths.RegressionTestPluginFile );
        Match match = Regex.Match(
            fileContents,
            @"public\s+const\s+string\s+VersionStr\s+=\s+""(?<version>\d+\.\d+\.\d+)"";"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Regression Test Version Regex" );
        }

        return match.Groups["version"].Value;
    }

    private string GetRegressionTestPluginName()
    {
        string fileContents = this.CakeContext.FileReadText( this.ImportantPaths.RegressionTestPluginFile );
        Match match = Regex.Match(
            fileContents,
            @"\[ChaskisPlugin\( ""(?<name>\w+)"" \)\]"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Chaskis Plugin Name Regex" );
        }

        return match.Groups["name"].Value;
    }

    private string GetDefaultPluginName()
    {
        string fileContents = this.CakeContext.FileReadText( this.ImportantPaths.DefaultPluginFile );
        Match match = Regex.Match(
            fileContents,
            @"internal\s+const\s+string\s+DefaultPluginName\s*=\s*""(?<name>\w+)"""
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Default Plugin Name Regex" );
        }

        return match.Groups["name"].Value;
    }

    private string GetTags()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<PackageTags\>(?<tags>.+)\</PackageTags\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Package Tags Regex" );
        }

        return match.Groups["tags"].Value;
    }

    private string GetAuthor()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<Authors\>(?<author>.+)\</Authors\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Author Regex" );
        }

        return match.Groups["author"].Value;
    }

    private string GetProjectUrl()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<PackageProjectUrl\>(?<PackageProjectUrl>.+)\</PackageProjectUrl\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Project URL Regex" );
        }

        return match.Groups["PackageProjectUrl"].Value;
    }

    private string GetLicenseUrl()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<PackageLicenseUrl\>(?<PackageLicenseUrl>.+)\</PackageLicenseUrl\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match License URL Regex" );
        }

        return match.Groups["PackageLicenseUrl"].Value;
    }

    private string GetCopyRight()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<Copyright\>(?<copyright>.+)\</Copyright\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Copyright Regex" );
        }

        return match.Groups["copyright"].Value;
    }

    private string GetDescription()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"(\<Description\>(?<description>.+)\</Description\>)",
            RegexOptions.Singleline
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Description Regex" );
        }

        return match.Groups["description"].Value;
    }

    private string GetReleaseNotes()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<PackageReleaseNotes\>(?<PackageReleaseNotes>.+)\</PackageReleaseNotes\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Release Notes Regex" );
        }

        return match.Groups["PackageReleaseNotes"].Value;
    }

    private string GetIconUrl()
    {
        string fileContents = this.chaskisCoreProjectContents;
        Match match = Regex.Match(
            fileContents,
            @"\<PackageIconUrl\>(?<PackageIconUrl>.+)\</PackageIconUrl\>"
        );
        if ( match.Success == false )
        {
            throw new ApplicationException( "Could not match Icon Url Regex" );
        }

        return match.Groups["PackageIconUrl"].Value;
    }

    private string GetDebChecksum()
    {
        string[] lines = this.CakeContext.FileReadLines( this.ImportantPaths.DebianChecksumFile );
        return lines[0];
    }

    private string GetMsiChecksum()
    {
        string[] lines = this.CakeContext.FileReadLines( this.ImportantPaths.MsiChecksumFile );
        return lines[0];
    }
}

public class FilesToTemplate
{
    // ---------------- Fields ----------------

    private readonly ImportantPaths paths;

    // ---------------- Constructor ----------------

    public FilesToTemplate( ImportantPaths paths )
    {
        this.paths = paths;

        List<FileToTemplate> files = new List<FileToTemplate>
        {
            // Windows WIX MSI Config.
            new FileToTemplate(
                this.paths.WixConfigFolder.CombineWithFilePath( new FilePath( "Product.wxs.template" ) ),
                this.paths.WixConfigFolder.CombineWithFilePath( new FilePath( "Product.wxs" ) ),
                new List<string>(){ "WINDOWS" }
            ),
            
            // Linux WIX MSI Config.
            new FileToTemplate(
                this.paths.WixConfigFolder.CombineWithFilePath( new FilePath( "Product.wxs.template" ) ),
                this.paths.WixConfigFolder.CombineWithFilePath( new FilePath( "Product.wxs.linux" ) ),
                new List<string>(){ "LINUX" }
            ),

            // Arch Linux PKGBUILD
            new FileToTemplate(
                this.paths.ArchLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "PKGBUILD.template" ) ),
                this.paths.ArchLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "PKGBUILD" ) )
            )
            {
                // Arch linux PKGBUILD must be \n, not \r\n.
                LineEnding = "\n"
            },

            // Debian Control File
            new FileToTemplate(
                this.paths.DebianLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "control.template" ) ),
                this.paths.DebianLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "control" ) )
            ),

            // Fedora Spec File
            new FileToTemplate(
                this.paths.FedoraLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "chaskis.spec.template" ) ),
                this.paths.FedoraLinuxInstallConfigFolder.CombineWithFilePath( new FilePath( "chaskis.spec" ) )
            ),

            // Chocolatey Nuspec
            new FileToTemplate(
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "template/chaskis.nuspec.template" ) ),
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "package/chaskis.nuspec" ) )
            ),

            // Chocolatey License
            new FileToTemplate(
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "template/tools/LICENSE.txt.template" ) ),
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "package/tools/LICENSE.txt" ) )
            ),

            // Chocolatey Install
            new FileToTemplate(
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "template/tools/chocolateyinstall.ps1.template" ) ),
                this.paths.ChocolateyInstallConfigFolder.CombineWithFilePath( new FilePath( "package/tools/chocolateyinstall.ps1" ) )
            ),

            // Root README
            new FileToTemplate(
                this.paths.ProjectRoot.CombineWithFilePath( new FilePath( "README.md.template" ) ),
                this.paths.ProjectRoot.CombineWithFilePath( new FilePath( "README.md" ) )
            ),

            // New Version Notifier
            new FileToTemplate(
                this.paths.RegressionTestFolder.CombineWithFilePath( new FilePath( "Environments/NewVersionNotifierNoChangeEnvironment/Plugins/NewVersionNotifier/.lastversion.txt.template" ) ),
                this.paths.RegressionTestFolder.CombineWithFilePath( new FilePath( "Environments/NewVersionNotifierNoChangeEnvironment/Plugins/NewVersionNotifier/.lastversion.txt" ) )
            ),
            
            // Chaskis Constants for Regression Tests
            new FileToTemplate(
                this.paths.RegressionTestFolder.CombineWithFilePath( new FilePath( "TestFixtures/TestCore/ChaskisConstants.cs.template" ) ),
                this.paths.RegressionTestFolder.CombineWithFilePath( new FilePath( "TestFixtures/TestCore/ChaskisConstants.cs" ) )
            ),
        };

        this.TemplateFiles = files.AsReadOnly();
    }

    // ---------------- Properties ----------------

    public IReadOnlyList<FileToTemplate> TemplateFiles { get; private set; }
}

public class FileToTemplate
{
    // ---------------- Constructor ----------------

    public FileToTemplate( FilePath input, FilePath output ) :
        this( input, output, null )
    {
    }

    public FileToTemplate( FilePath input, FilePath output, IReadOnlyList<string> defines )
    {
        this.Input = input;
        this.Output = output;

        if ( defines == null )
        {
            this.Defines = new List<string>();
        }
        else
        {
            this.Defines = defines;
        }

        this.LineEnding = null;
    }

    // ---------------- Properties ----------------

    public FilePath Input { get; private set; }

    public FilePath Output { get; private set; }

    public IReadOnlyList<string> Defines { get; private set; }

    /// <summary>
    /// After templating the file, all line endings
    /// with this string.
    ///
    /// Leave null to not replace.
    /// </summary>
    public string LineEnding { get; set; }
}

public class Templatizer
{
    // ---------------- Fields ----------------

    private readonly TemplateConstants constants;

    private readonly FilesToTemplate files;

    private readonly ICakeContext context;

    // ---------------- Constructor ----------------

    public Templatizer( TemplateConstants constants, FilesToTemplate filesToTemplate )
    {
        this.constants = constants;
        this.files = filesToTemplate;
        this.context = this.constants.CakeContext;
    }

    // ---------------- Functions ----------------

    public void Template()
    {
        foreach ( FileToTemplate file in this.files.TemplateFiles )
        {
            string contents;
            if ( file.Defines.Count == 0 )
            {
                // If we have no defines, just grab the whole file.
                contents = this.DoTemplate( this.context.FileReadText( file.Input ) );
            }
            else
            {
                // Otherwise, we need to be smart.   If something is defined,
                // include the text between the #IF and the #ENDIF.  Otherwise, move on.
                // This is bascially a terrible version of the C PreProcessor since WIX doesn't
                // like extra attributes.
                // This crappy version doesn't currently support nested if statements.
                List<Regex> ifRegexes = new List<Regex>();
                foreach( string define in file.Defines )
                {
                    ifRegexes.Add( new Regex( @"\s*#IF\s+" + define, RegexOptions.IgnoreCase ) );
                }

                Regex badRegex = new Regex( @"\s*#IF\s+\w+", RegexOptions.IgnoreCase );
                Regex endRegex = new Regex( @"\s*#ENDIF", RegexOptions.IgnoreCase );

                bool addLine = true;
                StringBuilder builder = new StringBuilder();
                foreach( string line in this.context.FileReadLines( file.Input ) )
                {
                    bool foundIfRegex = false;
                    foreach( Regex ifRegex in ifRegexes )
                    {
                        if( ifRegex.IsMatch( line ) )
                        {
                            foundIfRegex = true;
                            break;
                        }
                    }

                    if ( foundIfRegex )
                    {
                        addLine = true;
                        continue;
                    }
                    else if ( badRegex.IsMatch( line ) )
                    {
                        addLine = false;
                        continue;
                    }
                    else if ( endRegex.IsMatch( line ) )
                    {
                        addLine = true;
                        continue;
                    }
                    else if ( addLine )
                    {
                        builder.AppendLine( line );
                    }
                }
                contents = this.DoTemplate( builder.ToString() );
            }

            if( file.LineEnding != null )
            {
                contents = Regex.Replace( contents, Environment.NewLine, file.LineEnding );
            }

            this.context.FileWriteText( file.Output, contents );
        }
    }

    private string DoTemplate( string contents )
    {
        contents = Regex.Replace( contents, @"{%FullName%}", this.constants.FullName );
        contents = Regex.Replace( contents, @"{%ChaskisMainVersion%}", this.constants.ChaskisVersion );
        contents = Regex.Replace( contents, @"{%ChaskisCoreVersion%}", this.constants.ChaskisCoreVersion );
        contents = Regex.Replace( contents, @"{%License%}", this.constants.License );
        contents = Regex.Replace( contents, @"{%RegressionTestPluginVersion%}", this.constants.RegressionTestPluginVersion );
        contents = Regex.Replace( contents, @"{%RegressionTestPluginName%}", this.constants.RegressionTestPluginName );
        contents = Regex.Replace( contents, @"{%DefaultPluginName%}", this.constants.DefaultPluginName );
        contents = Regex.Replace( contents, @"{%CoreTags%}", this.constants.CoreTags );
        contents = Regex.Replace( contents, @"{%MainTags%}", this.constants.CliTags );
        contents = Regex.Replace( contents, @"{%Author%}", this.constants.Author );
        contents = Regex.Replace( contents, @"{%AuthorEmail%}", this.constants.AuthorEmail );
        contents = Regex.Replace( contents, @"{%ProjectUrl%}", this.constants.ProjectUrl );
        contents = Regex.Replace( contents, @"{%LicenseUrl%}", this.constants.LicenseUrl );
        contents = Regex.Replace( contents, @"{%WikiUrl%}", this.constants.WikiUrl );
        contents = Regex.Replace( contents, @"{%IssueTrackerUrl%}", this.constants.IssueTrackerUrl );
        contents = Regex.Replace( contents, @"{%CopyRight%}", this.constants.CopyRight );
        contents = Regex.Replace( contents, @"{%Description%}", this.constants.Description );
        contents = Regex.Replace( contents, @"{%ReleaseNotes%}", this.constants.ReleaseNotes );
        contents = Regex.Replace( contents, @"{%Summary%}", this.constants.Summary );
        contents = Regex.Replace( contents, @"{%IconUrl%}", this.constants.IconUrl );
        contents = Regex.Replace( contents, @"{%RunTime%}", this.constants.RunTime );
        contents = Regex.Replace( contents, @"{%DebCheckSum%}", this.constants.DebChecksum );
        contents = Regex.Replace( contents, @"{%MsiCheckSum%}", this.constants.MsiChecksum );

        return contents;
    }
}