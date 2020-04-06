using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npr, IMapper mapper)
        {
            _npRepo = npr;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();
            var objDto = new List<NationalParkDto>();

            foreach (var obj in objList)
            {
                objDto.Add(ConvertToDto(obj));
            }

            return Ok(objDto);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetNationalPark(int id)
        {
            var park = _npRepo.GetNationalPark(id);

            if (park == null)
            {
                return NotFound();
            }

            return Ok(ConvertToDto(park));
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Some4thing went wrong wehn saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
        }

        private NationalParkDto ConvertToDto(NationalPark park)
        {
            return _mapper.Map<NationalParkDto>(park);
        }
    }
}