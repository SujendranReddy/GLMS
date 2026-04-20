using GLMS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Xunit;

namespace GLMS.Tests
{
    public class FileServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _fileService = new FileService(_mockEnvironment.Object);
        }

        [Fact]
        public void IsPdf_ReturnsTrue_ForPdfFile()
        {
            var content = "Dummy PDF content";
            var fileName = "test.pdf";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            IFormFile file = new FormFile(stream, 0, stream.Length, "Data", fileName);

            var result = _fileService.IsPdf(file);

            Assert.True(result);
        }

        [Fact]
        public void IsPdf_ReturnsFalse_ForExeFile()
        {
            var content = "Dummy EXE content";
            var fileName = "virus.exe";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            IFormFile file = new FormFile(stream, 0, stream.Length, "Data", fileName);

            var result = _fileService.IsPdf(file);

            Assert.False(result);
        }

        [Fact]
        public void IsPdf_ReturnsFalse_ForNullFileName()
        {
            var content = "Dummy content";
            var fileName = "";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            IFormFile file = new FormFile(stream, 0, stream.Length, "Data", fileName);

            var result = _fileService.IsPdf(file);

            Assert.False(result);
        }
    }
}