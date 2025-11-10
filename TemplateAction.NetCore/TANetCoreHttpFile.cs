using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpFile : IRequestFile
    {
        private IFormFile _file;
        public TANetCoreHttpFile(IFormFile file)
        {
            _file = file;
        }
        public long ContentLength
        {
            get { return _file.Length; }
        }
        public string FileName
        {
            get { return _file.FileName; }
        }

        public string ContentType
        {
            get { return _file.ContentType; }
        }

        public Stream OpenReadStream()
        {
            return _file.OpenReadStream();
        }

        public void SaveAs(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                _file.CopyTo(stream);
            }
        }
        public async Task SaveAsAsync(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                await _file.CopyToAsync(stream);
            }
        }
    }
}
