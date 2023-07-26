using FileUpload.data;

namespace FileUploadDemo.web.Models
{
    public class ViewImageViewModel
    {
        public Picture Pic { get; set; }

        public bool HasSeenThisOne { get; set; }

        public string IncorrectPassword { get; set; }
    }
}
