using build.ironscheme.net.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace build.ironscheme.net.Controllers
{
  public class HomeController : Controller
  {
    static readonly Regex FILENAME =
      new Regex(@"(?<filename>IronScheme-(?<version>(\d|[a-f])+))\.(?<ext>(zip|(tar\.xz)))", RegexOptions.Compiled);

    public ActionResult Index()
    {
      var q = GetBuilds();
      return View(q);
    }

    public ActionResult GetLatest(string type = "zip")
    {
      var latest = GetBuilds().Latest;
      ArchiveInfo ai = null;
      if (latest.Variations.TryGetValue(type, out ai))
      {
        return Redirect(ai.FullName);
      }
      return new EmptyResult();
    }

    Model GetBuilds()
    {
      int dummy;
      var q = from fn in Directory.EnumerateFiles(Server.MapPath("~"))
              let file = Path.GetFileName(fn)
              let m = FILENAME.Match(file)
              where m.Success
              group new { file, fn, m } by m.Groups["filename"].Value into f
              let e = f.First()
              let ver = e.m.Groups["version"].Value
              let isgit = !int.TryParse(ver, out dummy)
              select new BuildInfo
              {
                Name = f.Key,
                Version = ver,
                IsGit = isgit,
                Checkin = (isgit ? "https://github.com/leppie/IronScheme/commit/" : "http://ironscheme.codeplex.com/SourceControl/changeset/") + ver,
                Date = new FileInfo(e.fn).LastWriteTimeUtc,
                Variations = f.ToDictionary(
                  x => x.m.Groups["ext"].Value,
                  x => new ArchiveInfo
                  {
                    FullName = x.file,
                    Size = new FileInfo(x.fn).Length / 1024
                  })
              }
                into f
              orderby f.Date descending
              select f;
      return new Model { Builds = q };
    }
  }
}
