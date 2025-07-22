using AutoMapper;
using Common.DTO;
using LatexCompiler.Entities;

namespace LatexCompiler.Automapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<LatexProject, LatexProjectDTO>().ReverseMap();
    }
}