using System;
using System.Web.Mvc;
using FileNotes.Web.Models.NoteModel;

namespace FileNotes.Web.Controllers
{
    public class NoteController : Controller
    {
        private readonly NoteFacade _noteFacade;

        public NoteController(NoteFacade noteFacade)
        {
            if (noteFacade == null) throw new ArgumentNullException("noteFacade");
            _noteFacade = noteFacade;
        }

        [HttpGet]
        public ActionResult Show(int id)
        {
            return View(_noteFacade.GetForShow(id));
        }
    }
}