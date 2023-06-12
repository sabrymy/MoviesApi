using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesApi.DTOs;
using MoviesApi.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre,GenreDTO>();
            CreateMap<Genre, GenreCreationDTO>().ReverseMap();
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<PersonCreationDTO, Person>().ReverseMap()
               .ForMember(x => x.picture,memberOptions=> memberOptions.Ignore()) ;
            CreateMap<Person, PersonPatchDTO>().ReverseMap();
            CreateMap<Movie, MovieDTO>().ReverseMap();

            CreateMap<MovieCreationDTO, Movie>()
               .ForMember(x => x.Poster, options => options.Ignore())
               .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
               .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors))
              ;

            CreateMap<   Movie, MoviesDetailsDTO > ()
              .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
              .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors))
             ;

            CreateMap<Movie, MoviePatchDTO>().ReverseMap();
            //FOR MEMBER FN WORK ON DESTINATION PARAMETER  MAP FROM FN ON SOURCE PARAM
            CreateMap<IdentityUser, UserDTO>()
                .ForMember(x => x.EmailAddress, options => options.MapFrom(x => x.Email))
                .ForMember(x => x.UserId, options => options.MapFrom(x => x.Id));


        }
        private List<GenreDTO> MapMoviesGenres(Movie movie, MoviesDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();
            foreach (var moviegenre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = moviegenre.GenreId, Name = moviegenre.Genre.Name });
            }
            return result;
        }

        private List<ActorDTO> MapMoviesActors( Movie movie, MoviesDetailsDTO moviesDetailsDTO)
        {   
            var result = new List<ActorDTO>();
            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDTO() { PersonId = actor.PersonId , Character= actor.Character, PersonName= actor.Person.Name });
            }
            return result;
        }





        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { PersonId = actor.PersonId, Character = actor.Character });
            }
            return result;
        }


    }
}
