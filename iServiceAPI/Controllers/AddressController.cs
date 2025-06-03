using iServiceRepositories.Repositories.Models;
using iServiceServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(IConfiguration configuration)
        {
            _addressService = new AddressService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<List<Address>>> Get()
        {
            var result = await _addressService.GetAllAddresses();

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpGet("{addressId}")]
        public async Task<ActionResult<Address>> GetById(int addressId)
        {
            var result = await _addressService.GetAddressById(addressId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(new { message = result.ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Post([FromBody] Address addressModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _addressService.AddAddress(addressModel);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { addressId = result.Value.AddressId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPost("Save")]
        public async Task<ActionResult<UserInfo>> Save([FromBody] UserInfo request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _addressService.SaveAddress(request);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { addressId = result.Value.Address.AddressId }, result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{addressId}")]
        public async Task<ActionResult<Address>> Put(int addressId, [FromBody] Address address)
        {
            if (addressId != address.AddressId)
            {
                return BadRequest();
            }

            var result = await _addressService.UpdateAddress(address);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpPut("{addressId}/SetActive")]
        public async Task<ActionResult<bool>> SetActive(int addressId, [FromBody] bool isActive)
        {
            var result = await _addressService.SetActiveStatus(addressId, isActive);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        [HttpDelete("{addressId}/SetDeleted")]
        public async Task<ActionResult<bool>> SetDeleted(int addressId, [FromBody] bool isDeleted)
        {
            var result = await _addressService.SetDeletedStatus(addressId, isDeleted);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
    }
}