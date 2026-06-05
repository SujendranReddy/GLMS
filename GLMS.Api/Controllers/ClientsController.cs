using GLMS.DTOs;
using GLMS.Interfaces;
using GLMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();

            var clientDtos = clients
                .Select(MapToDto)
                .ToList();

            return Ok(clientDtos);
        }

        // GET: api/clients/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(client));
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient(
            [FromBody] CreateClientDto createClientDto)
        {
            var client = new Client
            {
                Name = createClientDto.Name,
                Email = createClientDto.Email,
                PhoneNumber = createClientDto.PhoneNumber,
                Region = createClientDto.Region
            };

            var createdClient = await _clientRepository.CreateAsync(client);
            var result = MapToDto(createdClient);

            return CreatedAtAction(
                nameof(GetClientById),
                new { id = result.ClientId },
                result);
        }

        // PUT: api/clients/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(
            int id,
            [FromBody] UpdateClientDto updateClientDto)
        {
            var client = new Client
            {
                ClientId = id,
                Name = updateClientDto.Name,
                Email = updateClientDto.Email,
                PhoneNumber = updateClientDto.PhoneNumber,
                Region = updateClientDto.Region
            };

            var updated = await _clientRepository.UpdateAsync(client);

            if (!updated)
            {
                return NotFound();
            }

            var savedClient = await _clientRepository.GetByIdAsync(id);

            return Ok(MapToDto(savedClient!));
        }

        // DELETE: api/clients/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var deleted = await _clientRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static ClientDto MapToDto(Client client)
        {
            return new ClientDto
            {
                ClientId = client.ClientId,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Region = client.Region
            };
        }
    }
}