using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Dotnet_4_V2.Server.DTO.Rooms;
using Project_Dotnet_4_V2.Server.Services;
using Projet_Dotnet_4.DTO.Rooms;
using Projet_Dotnet_4.Models;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoomById(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom(CreateRoomDTO roomDto)
    {
        var room = await _roomService.CreateRoomAsync(roomDto);
        return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> ModifyRoom(int id, ModifyRoomDTO roomDto)
    {
        if (id != roomDto.Id)
            return BadRequest("ID mismatch");

        try
        {
            await _roomService.UpdateRoomAsync(id, roomDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(int id)
    {
        try
        {
            await _roomService.DeleteRoomAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}