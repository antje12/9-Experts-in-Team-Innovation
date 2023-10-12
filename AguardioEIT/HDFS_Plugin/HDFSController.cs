using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HDFS_Plugin
{
    [ApiController]
    [Route("[controller]")]
    public class HDFSController : ControllerBase
    {
        private readonly IHDFS_Service _hdfsService;

        public HDFSController(IHDFS_Service hdfsService)
        {
            _hdfsService = hdfsService;
        }

        [HttpGet("GetFile")]
        public object GetFile()
        {
            return "file.txt";
        }
        
        [HttpPost("SaveFile")]
        public object SaveFile()
        {
            return "Saved";
        }
    }
    
}

