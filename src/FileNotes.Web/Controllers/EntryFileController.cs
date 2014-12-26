using System;
using System.Web.Mvc;
using FileNotes.Web.Models.EntryFileModel;

namespace FileNotes.Web.Controllers
{
    public class EntryFileController : Controller
    {
        private readonly EntryFileFacade _entryFileFacade;

        public EntryFileController(EntryFileFacade entryFileFacade)
        {
            if (entryFileFacade == null) throw new ArgumentNullException("entryFileFacade");
            _entryFileFacade = entryFileFacade;
        }

        [HttpGet]
        public ActionResult All()
        {
            return View();
        }

        [HttpGet]
        public JsonResult AllData(InputEntriesVm entriesVm)
        {
            var vm = _entryFileFacade.GetAll(entriesVm);
            var result = Json(vm, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}
