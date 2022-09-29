using AutoMapper;
using Bankid.Interfaces;
using Bankid.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankid.Controllers {
    public class CoursesController : BaseApiController {
        private readonly ICurrentUser _currentUser;
        public CoursesController(IMapper mapper, Data.AppDbContext dbContext, ICurrentUser currentUser) : base(dbContext, mapper) {
            _currentUser = currentUser;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/courses")]
        public async Task<IActionResult> GetAllAsync() {
            var courses = await DbContext.Courses.AsQueryable().OrderBy(x => x.Title).ToListAsync();
            if (_currentUser.Role != Role.AdminRoleName) {
                courses = courses.Where(x => !x.OnlyAdmin).ToList();
            }
            return Ok(courses);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/courses/{courseId}")]
        public async Task<IActionResult> Get(int courseId) {
            var course = await DbContext.Courses.FindAsync(courseId);
            return Content(course.Html, "text/html", Encoding.UTF8);
        }



        [HttpDelete]
        [Route("/api/courses/{courseId}")]
        public async Task<IActionResult> DeleteAsync(int courseId) {
            DbContext.Courses.Remove(await DbContext.Courses.FindAsync(courseId));
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("/api/courses")]
        public async Task<IActionResult> AddAsync(Course course) {
            if (_currentUser.Role != Role.AdminRoleName) return Forbid();
            course.Id = 0;
            course.AuthorId = _currentUser.UserId;
            course.Author = null;
            course.CreatedDate = System.DateTime.UtcNow;

            await DbContext.Courses.AddAsync(course);
            DbContext.SaveChanges();

            return Ok();
        }
    }
}
