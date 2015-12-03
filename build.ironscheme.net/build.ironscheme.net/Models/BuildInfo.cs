using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace build.ironscheme.net.Models
{
  public class BuildInfo
  {
    public string Name { get; set; }
    public string Version { get; set; }
    public DateTime Date { get; set; }
    public string Checkin { get; set; }
    public Dictionary<string, ArchiveInfo> Variations { get; set; }
    public bool IsGit { get; set; }
  }

  public class ArchiveInfo
  {
    public string FullName { get; set; }
    public long Size { get; set; } 
  }

  public class Model
  {
    public IEnumerable<BuildInfo> Builds { get; set; }

    BuildInfo latest;
    
    public BuildInfo Latest
    {
      get
      {
        return latest ?? (latest = Builds.First());
      }
    }
  }
}