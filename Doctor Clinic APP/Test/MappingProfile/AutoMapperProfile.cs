using AutoMapper;
using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;



namespace Doctor_CLinic_API.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Map User to UserDTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.AppointmentIds, opt => opt.MapFrom(src => src.Appointments.Select(a => a.Id)))
                .ForMember(dest => dest.PatientIds, opt => opt.MapFrom(src => src.Patients.Select(p => p.Id)));

            // Map UserDTO to User
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Appointments, opt => opt.Ignore())
                .ForMember(dest => dest.Patients, opt => opt.Ignore());

            // Mapping for RegisterDTO and User
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<LoginDTO, User>();

            // Mapping for Patient and PatientDTO
            CreateMap<Patient, PatientDTO>()
                .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments));

            CreateMap<PatientDTO, Patient>();

            // Mapping for Appointment and AppointmentDTO
            CreateMap<Appointment, AppointmentDTO>();
            CreateMap<AppointmentDTO, Appointment>();
        }
    }

}
