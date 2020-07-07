// Pipeline to make a new release.
// This Pipeline requires the following be installed on a WINDOWS 10 PC (Install the following Chocolatey Packages):
// - docker-desktop
// - docker-cli
// - dotnetcore-sdk

// The following Windows features must be turned on:
// - Containers
// - Hyper-V
// - OpenSSH Server (Search for Optional Features in the Windows 10).  Can also install with Chocolatey via "choco install openssh -params "/SSHServerFeature /SSHAgentFeature""
//   This is so Jenkins can remote into the Windows machine to build.

// Jenkins Agent User Config:
// - The user Jenkins will SSH into the Windows machine as must be in the docker-users and adminstrator group.
//   If the user is not in the administrator group, dockercli.exe will not work in an SSH environment.  I have no idea why, it just won't.
// - Generate an SSH key with "ssh key-gen".  Make sure the public key is in the user's .ssh/authorized_keys file.
// - In c:\Program Data\ssh\sshd_config, comment out the following lines at the bottom (or make it work, not sure how though).
//   #Match Group administrators
//   #       AuthorizedKeysFile __PROGRAMDATA__/ssh/administrators_authorized_keys
//
//   If this is uncommented, Jenkins will NOT be able to SSH in as your user; ssh wil reject it.  This is because the user Jenkins SSHes into
//   is in the admin group.
//   ... Docker for Windows is not a fun experience.

// Power Settings (Possibly not needed)
// Yes, we even need to change the Power Settings because Windows tries to be "helpful" but can put Docker into a bad state.
// This is to workaround a problem called out here: https://stackoverflow.com/questions/40668908/running-docker-for-windows-error-when-exposing-ports
// - Hit start and search for "Power".  Click on "Power & Sleep" settings.
// - On the right column, hit "Additional Power Settings".
// - On the Window that pops up, hit "Choose what the power buttons do"
// - Uncheck "Turn off fast startup"

// Before running
// - Ensure the Docker Engine is running by going to Start -> searching for "Services" and hitting enter.
//   You can have it start automatically by setting the Startup Type to "Automatic".
// - Also make sure the "Docker Desktop Service" is also running.

def distFolder = "dist"; // Staging to before we push to the website.
def buildDistFolder = ".\\Chaskis\\DistPackages"; // Where builds post their dists.
def archiveFolder = "archives";
def websiteCreds = "shendrick.net";
def aurCreds = "aur";

def UbuntuBuildImageName()
{
    return "chaskis.build.ubuntu";
}

def ArchBuildImageName()
{
    return "chaskis.build.archlinux";
}

def UseWindowsDockerForBuild()
{
    // Can't quite get the Windows build container working to generate .MSI's.  May need to wait for the next generation of WiX to be
    // built first.  The Windows PC has everything installed anyways, so meh.
    //
    // This needs to be a function since Jenkins is too stupid to read global variables.
    return false;
}

def WindowsSleep( Integer seconds )
{
    echo "Sleeping for ${seconds} seconds...";
    // This is apparently the only reliable way to sleep on a Windows PC >_>.
    bat "ping -n ${seconds} 127.0.0.1";
    echo "Sleeping for ${seconds} seconds... Done!";
}

///
/// Calls cake in the Windows docker container (or locally if there is no Docker container for Windows.)
///
def CallCakeOnWindows( String cmd )
{
    if( UseWindowsDockerForBuild() )
    {
        bat "docker run --mount type=bind,source=\"${pwd()}\\Chaskis\",target=c:\\Chaskis -i chaskis.build.windows c:\\Chaskis\\build.cake ${cmd}";

        // Sleep for 5 seconds to give containers a chance to cleanup.
        WindowsSleep( 5 );
    }
    else
    {
        bat ".\\Cake\\dotnet-cake.exe .\\Chaskis\\build.cake ${cmd}"
    }
}

///
/// Calls cake in a Linux docker container.
///
def CallCakeOnUbuntu( String cmd )
{
    // Mount to the container user's home directory so there are no permission issues.
    bat "docker run --mount type=bind,source=\"${pwd()}\\Chaskis\",target=/home/ContainerUser/chaskis/ -i ${UbuntuBuildImageName()} /home/ContainerUser/chaskis/build.cake ${cmd}"

    // Sleep for 5 seconds to give containers a chance to cleanup.
    WindowsSleep( 5 );
}

///
/// Calls cake in the Arch Linux docker container.
///
def CallCakeOnArchLinux( String cmd )
{
    // Mount to the container user's home directory so there are no permission issues.
    bat "docker run --mount type=bind,source=\"${pwd()}\\Chaskis\",target=/home/ContainerUser/chaskis/ -i ${ArchBuildImageName()} /home/containeruser/chaskis/build.cake ${cmd}"

    // Sleep for 5 seconds to give containers a chance to cleanup.
    WindowsSleep( 5 );
}

///
/// Calls cake on the Jenkins Agent, with no docker container.
///
def CallCakeOnBuildMachine( String cmd )
{
    bat ".\\Cake\\dotnet-cake.exe .\\Chaskis\\build.cake ${cmd}"
}

def CleanWindowsDirectory( String path )
{
    bat "IF EXIST \"${path}\" rmdir /S /Q \"${path}\""
    bat "mkdir \"${path}\""
}

def RunProcessIgnoreError( String cmd )
{
    try
    {
        bat cmd
    }
    catch( Exception )
    {

    }
}

def GetVersFile()
{
    return "version.txt";
}

def GetChaskisVersion()
{
    return readfile( GetVersFile() );
}

def ParseTestResults( String filePattern )
{
    def results = xunit thresholds: [
        failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
    ], tools: [
        MSTest(
            deleteOutputFiles: true,
            failIfNotNew: true,
            pattern: filePattern,
            skipNoTestFiles: true,
            stopProcessingIfError: true
        )
    ]
}

def PostDirectoryToWebsite( String localDirectory )
{
    withCredentials(
        [sshUserPrivateKey(
            credentialsId: websiteCreds,
            usernameVariable: "ssh_user"
        )]
    )
    {
        def releaseLocation = "/home/${ssh_user}/files.shendrick.net/projects/chaskis/releases/${GetChaskisVersion()}/";
        sshagent(credentials:[websiteCreds])
        {
            // Upload everything to the release folder.
            bat "scp -r -o StrictHostKeyChecking=no ${localDirectory} ${ssh_user}@shendrick.net:${releaseLocation}";

            // Make all uploaded directories 755
            bat "ssh -o StrictHostKeyChecking=no ${ssh_user}@shendrick.net find ${releaseLocation} -type d -exec chmod 755 {} +";

            // Make all uploaded files 644
            bat "ssh -o StrictHostKeyChecking=no ${ssh_user}@shendrick.net find ${releaseLocation} -type f -exec chmod 644 {} +"
        }
    }
}

def GitPush( String repoLocation, String credId )
{
    withCredentials(
        [sshUserPrivateKey(
            credentialsId: credId,
            usernameVariable: "ssh_user"
        )]
    )
    {
        sshagent(credentials:[credId])
        {
            bat "cd ${repoLocation} && git push";
        }
    }
}

pipeline
{
    agent
    {
        label "windows";
    }
    parameters
    {
        booleanParam( name: "BuildWindows", defaultValue: true, description: "Should we build for Windows?" );
        booleanParam( name: "BuildLinux", defaultValue: true, description: "Should we build for Linux?" );
        booleanParam( name: "RunRegressionTests", defaultValue: true, description: "Should regression tests be run?" );
        booleanParam( name: "Deploy", defaultValue: true, description: "Should we deploy?" );
    }
    stages
    {
        stage( 'checkout' )
        {
            steps
            {
                checkout poll: false, scm: [
                    $class: 'GitSCM', 
                    branches: [[name: '*/master']],
                    doGenerateSubmoduleConfigurations: false,
                    extensions: [
                        [$class: 'CleanBeforeCheckout'],
                        [$class: 'RelativeTargetDirectory', relativeTargetDir: 'Chaskis'],
                        [$class: 'CloneOption', depth: 0, noTags: true, reference: '', shallow: true],
                        [$class: 'SubmoduleOption', disableSubmodules: false, parentCredentials: false, recursiveSubmodules: true, reference: '', trackingSubmodules: false]
                    ],
                    submoduleCfg: [],
                    userRemoteConfigs: [[url: 'https://github.com/xforever1313/Chaskis.git']]
                ]
            }
        }
        stage( 'setup' )
        {
            steps
            {
                bat "dotnet tool update Cake.Tool --tool-path .\\Cake"
                CallCakeOnBuildMachine( "--showdescription" );
                CleanWindowsDirectory( pwd() + "\\${distFolder}" );
                CleanWindowsDirectory( pwd() + "\\${archiveFolder}" );
                CallCakeOnBuildMachine( "--target=dump_version --output=\"${pwd()}\\${GetVersFile()}\"" );
            }
        }
        stage( 'Windows' )
        {
            stages
            {
                stage( 'make_build_container' )
                {
                    steps
                    {
                        // Make the Windows build docker container.
                        bat 'C:\\"Program Files"\\Docker\\Docker\\DockerCli.exe -Version';
                        bat 'C:\\"Program Files"\\Docker\\Docker\\DockerCli.exe -SwitchWindowsEngine';
                        WindowsSleep( 10 ); // Wait to switch.
                        script
                        {
                            if( UseWindowsDockerForBuild() )
                            {
                                bat "docker build -t chaskis.build.windows -f .\\Chaskis\\Docker\\WindowsBuild.Dockerfile .\\Chaskis";
                            }
                        }
                    }
                }
                stage( 'build_debug' )
                {
                    steps
                    {
                        // Build Debug
                        CallCakeOnWindows( "--target=debug" );
                    }
                }
                stage( 'unit_test' )
                {
                    steps
                    {
                        CallCakeOnWindows( "--target=unit_test" );
                    }
                    post
                    {
                        always
                        {
                            ParseTestResults( "Chaskis\\TestResults\\UnitTests\\*.xml" );
                        }
                    }
                }
                stage( 'regression_test' )
                {
                    steps
                    {
                        CallCakeOnWindows( "--target=regression_test" );
                    }
                    when
                    {
                        expression
                        {
                            return params.RunRegressionTests;
                        }
                    }
                    post
                    {
                        always
                        {
                            ParseTestResults( "Chaskis\\TestResults\\RegressionTests\\*.xml" );
                            bat "MOVE .\\Chaskis\\TestResults\\RegressionTests\\Logs .\\${archiveFolder}\\windows_regression_logs";
                            archiveArtifacts "${archiveFolder}\\windows_regression_logs\\*.log";
                        }
                    }
                }
                stage( 'build_msi' )
                {
                    steps
                    {
                        CallCakeOnWindows( "--target=msi" );
                    }
                }
                stage( 'nuget_pack' )
                {
                    steps
                    {
                        CallCakeOnWindows( "--target=nuget_pack" );
                    }
                }
                stage( 'choco_pack' )
                {
                    steps
                    {
                        // Chocolatey needs the checksum to be updated.
                        // This file will be reverted during the Linux stage, but restored later during the deploy stage.
                        bat "COPY /Y ${buildDistFolder}\\windows\\ChaskisInstaller.msi.sha256 .\\Chaskis\\SavedChecksums\\ChaskisInstaller.msi.sha256"
                        CallCakeOnBuildMachine( "--target=template" );
                        CallCakeOnWindows( "--target=choco_pack" );
                    }
                }
                stage( 'make_runtime_container' )
                {
                    steps
                    {
                        bat ".\\Cake\\dotnet-cake.exe .\\Chaskis\\build.cake --target=build_windows_docker"
                    }
                }
                stage( 'Archive' )
                {
                    steps
                    {
                        // No need for Docker to stick around, delete.
                        bat "rmdir /S /Q ${buildDistFolder}\\windows\\docker"

                        // Move binaries over so they can be posted online.
                        bat "MOVE ${buildDistFolder}\\windows .\\${distFolder}\\windows"
                        bat "MOVE ${buildDistFolder}\\chocolatey .\\${distFolder}\\chocolatey"
                        bat "MOVE ${buildDistFolder}\\nuget .\\${distFolder}\\nuget"
                    }
                }
            }
            when
            {
                expression
                {
                    return params.BuildWindows;
                }
            }
        }// End Windows
        stage( "Linux" )
        {
            stages
            {
                stage( 'setup' )
                {
                    steps
                    {
                        bat "cd Chaskis && git clean -dfx"
                    }
                }
                stage( 'Make Linux Build Container' )
                {
                    steps
                    {
                        // Make the Linux build docker container.
                        bat 'C:\\"Program Files"\\Docker\\Docker\\DockerCli.exe -SwitchLinuxEngine';
                        WindowsSleep( 10 ); // Give some time to switch.
                        bat "docker build -t ${UbuntuBuildImageName()} -f .\\Chaskis\\Docker\\UbuntuBuild.Dockerfile .\\Chaskis";
                    }
                }
                stage( 'build_debug' )
                {
                    steps
                    {
                        // Build Debug
                        CallCakeOnUbuntu( "--target=debug" );
                    }
                }
                stage( 'unit_test' )
                {
                    steps
                    {
                        CallCakeOnUbuntu( "--target=unit_test" );
                    }
                    post
                    {
                        always
                        {
                            ParseTestResults( "Chaskis\\TestResults\\UnitTests\\*.xml" );
                        }
                    }
                }
                stage( 'regression_test' )
                {
                    steps
                    {
                        CallCakeOnUbuntu( "--target=regression_test" );
                    }
                    when
                    {
                        expression
                        {
                            return params.RunRegressionTests;
                        }
                    }
                    post
                    {
                        always
                        {
                            ParseTestResults( "Chaskis\\TestResults\\RegressionTests\\*.xml" );
                            bat "MOVE .\\Chaskis\\TestResults\\RegressionTests\\Logs .\\${archiveFolder}\\linux_regression_logs";
                            archiveArtifacts "${archiveFolder}\\linux_regression_logs\\*.log";
                        }
                    }
                }
                stage( 'build_release' )
                {
                    steps
                    {
                        CallCakeOnUbuntu( "--target=release" );
                    }
                }
                stage( 'make_deb' )
                {
                    steps
                    {
                        CallCakeOnUbuntu( "--target=debian_pack --deb_build_dir=/home/ContainerUser/deb" );
                    }
                }
                stage( 'make_runtime_container' )
                {
                    steps
                    {
                        bat ".\\Cake\\dotnet-cake.exe .\\Chaskis\\build.cake --target=build_ubuntu_docker";
                    }
                }
                stage( 'Archive' )
                {
                    steps
                    {
                        // Move binaries over so they can be posted online.
                        bat "MOVE ${buildDistFolder}\\debian .\\${distFolder}\\debian"
                    }
                }
            }
            when
            {
                expression
                {
                    return params.BuildLinux;
                }
            }
        } // End Linux
        stage( 'deploy' )
        {
            stages
            {
                stage( 'save_checksums' )
                {
                    steps
                    {
                        // When deploying, we need to make sure all checksums are up-to-date.
                        bat "COPY /Y ${distFolder}\\windows\\ChaskisInstaller.msi.sha256 .\\Chaskis\\SavedChecksums\\ChaskisInstaller.msi.sha256";
                        bat "COPY /Y ${distFolder}\\debian\\chaskis.deb.sha256 .\\Chaskis\\SavedChecksums\\chaskis.deb.sha256";
                        CallCakeOnBuildMachine( "--target=template" );
                    }
                }
                stage( 'upload_to_website' )
                {
                    steps
                    {
                        // PKGBUILD for Arch Linux requires files be deployed to the website first.
                        PostDirectoryToWebsite( distFolder );
                    }
                }
                stage( 'Arch Linux Deploy' )
                {
                    steps
                    {
                        // Build the Arch Linux Docker image so we can make the PKGBUILD file.
                        // Because arch sources change *all the time*, need to build from scratch.
                        bat "docker build -t ${ArchBuildImageName()} -f .\\Chaskis\\Docker\\ArchLinux.Dockerfile .\\Chaskis";

                        // Run Cake in the docker image to make the PKGBUILD file.
                        CallCakeOnArchLinux( "--target=pkgbuild" ); 

                        // Move binaries over so they can be posted online.
                        bat "MOVE ${buildDistFolder}\\arch_linux .\\${distFolder}\\arch_linux";
                        PostDirectoryToWebsite( ".\\${distFolder}\\arch_linux" );

                        // Clone the AUR git repo.
                        checkout poll: false, scm: [
                            $class: 'GitSCM', 
                            branches: [[name: '*/master']],
                            doGenerateSubmoduleConfigurations: false,
                            extensions: [
                                [$class: 'CleanBeforeCheckout'],
                                [$class: 'RelativeTargetDirectory', relativeTargetDir: 'chaskis_aur'],
                                [$class: 'CloneOption', depth: 0, noTags: true, reference: '', shallow: true],
                                [$class: 'SubmoduleOption', disableSubmodules: false, parentCredentials: false, recursiveSubmodules: true, reference: '', trackingSubmodules: false]
                            ],
                            submoduleCfg: [],
                            userRemoteConfigs: [[credentialsId: aurCreds, url: 'ssh://aur@aur.archlinux.org/chaskis.git']]
                        ]

                        // Copy over the SRCINFO and PKGBUILD files.
                        bat "COPY /Y ${distFolder}\\arch_linx\\.SRCINFO .\\chaskis_aur\\.SRCINFO";
                        bat "COPY /Y ${distFolder}\\arch_linx\\PKGBUILD .\\chaskis_aur\\PKGBUILD";

                        // Commit && Push.
                        bat "cd chaskis_aur && git commit -a -m \"Deployed Version ${GetChaskisVersion()}\"";
                        //GitPush( "chaskis_aur", aurCreds );
                    }
                }
                stage( 'Commit' )
                {
                    steps
                    {
                        bat "cd Chaskis && git commit -a -m \"Deployed Version ${GetChaskisVersion()}\"";
                        // GitPush( "Chaskis", websiteCreds );
                    }
                }
                stage( 'Deploy NuGet' )
                {
                    steps
                    {
                        withCredentials([string(credentialsId: 'chaskiscore_nuget_deploy', variable: 'nuget_api_key')])
                        {
                            // bat "nuget push ${distFolder}\\nuget\\*.nupkg ${nuget_api_key}";
                            WindowsSleep( 1 );
                        }
                    }
                }
                stage( 'Deploy Chocolatey' )
                {
                    steps
                    {
                        withCredentials([string(credentialsId: 'choco_api_key', variable: 'choco_api_key')])
                        {
                            // bat "choco push ${distFolder}\\chocolatey\\*.nupkg -k ${choco_api_key}";
                            WindowsSleep( 1 );
                        }
                    }
                }
            }
            when
            {
                expression
                {
                    return params.Deploy;
                }
            }
        } // End Deploy
    }
}