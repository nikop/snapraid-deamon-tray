using System.Collections.Generic;

namespace SnapraidDaemonTray.Config;

public class ConfigFile
{
    public List<ConfigFileServer> Servers { get; set; } = new List<ConfigFileServer>();
}
