using Microsoft.AspNetCore.Mvc;

public class AWSFilesController : Controller
{
    private ServiceStorageS3 service;

    public AWSFilesController(ServiceStorageS3 service)
    {
        this.service = service;
    }

    public async Task<IActionResult> Index()
    {
        List<string> filesS3 = await
            this.service.GetVersionsFileAsync();
        return View(filesS3);
    }

    public IActionResult UploadFile()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile
        (IFormFile file)
    {
        using (Stream stream = file.OpenReadStream())
        {
            await this.service.UploadFileAsync
                (file.FileName, stream);
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DeleteFile(string filename)
    {
        await this.service.DeleteFileAsync(filename);
        return RedirectToAction("Index");
    }
}
