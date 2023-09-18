using AutoMapper;
using CarRenting.BusinessObjects.Models;
using CarRenting.DTOs;
using CarRenting.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace CarRenting.Repositories.Repo;

public class CarInformationRepo : IBaseRepo<CarInformationDto>
{
    private readonly FUCarRentingManagementContext _context;
    private readonly IMapper _mapper;

    public CarInformationRepo()
    {
        _context = new FUCarRentingManagementContext();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CarInformationDto, CarInformation>();
            cfg.CreateMap<CarInformation, CarInformationDto>();
            cfg.CreateMap<List<CarInformationDto>, List<CarInformation>>();
        });

        _mapper = new Mapper(config);
    }

    public async Task<List<CarInformationDto>> GetAsync()
    {
        var entities = await _context.CarInformations.ToListAsync();
        return _mapper.Map<List<CarInformationDto>>(entities);
    }

    public async Task<CarInformationDto> AddAsync(CarInformationDto dataDto)
    {
        var entity = _mapper.Map<CarInformation>(dataDto);
        var maxId = await _context.CarInformations.MaxAsync(o => o.CarId);
        entity.CarId = maxId + 1;
        var rEntry = await _context.CarInformations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<CarInformationDto>(rEntry);
    }

    public async Task<CarInformationDto?> GetByIdAsync(int id)
    {
        var entity = await _context.CarInformations.FirstOrDefaultAsync(od => od.CarId == id);
        if (entity == null)
        {
            return null;
        }

        return _mapper.Map<CarInformationDto>(entity);
    }

    public async Task<CarInformationDto?> UpdateAsync(CarInformationDto dataDto)
    {
        var entity = _context.CarInformations.FirstOrDefault(od => od.CarId == dataDto.CarId);
        if (entity != null)
        {
            _context.Entry(entity).CurrentValues.SetValues(_mapper.Map<CarInformation>(dataDto));
            await _context.SaveChangesAsync();
            return _mapper.Map<CarInformationDto>(entity);
        }

        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var carInformation = await _context.CarInformations.FirstOrDefaultAsync(od => od.CarId == id);
        if (carInformation != null)
        {
            _context.CarInformations.Remove(carInformation);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}