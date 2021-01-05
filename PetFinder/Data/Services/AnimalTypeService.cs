﻿using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class AnimalTypeService : IAnimalTypeService
    {
        private readonly PetFinderContext _context;
        private readonly ILogger _logger;
        public const string INVALID_NAME_ERROR = "El nombre ingresado no corresponde a un nombre valido";
        public const string REPEATED_ANIMAL_TYPE_ERROR = "Ya existe el tipo de animal";
        public const string SAVING_ERROR = "Ocurrio un error al guardar";

        public AnimalTypeService(   PetFinderContext context, 
                                    ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Delete(int id)
        {
            var animalType = await Get(id);
            _context.Remove(animalType);
            _logger.Warning("AnimalType {name} deleted, Id: {id}", animalType.Name, animalType.Id);
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<IEnumerable<AnimalType>> GetAll()
        {
            return await _context.AnimalTypes.ToListAsync();
        }

        public async Task<AnimalType> Get(int Id)
        {
            return await _context.AnimalTypes.FindAsync(Id);
        }

        public async Task<bool> Insert(AnimalType animalType)
        {
            _context.AnimalTypes.Add(animalType);
            _logger.Information("AnimalType {name} created", animalType.Name);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(AnimalType animalType)
        {
            //Le decimos a la DB que el animalType fue modificado
            _context.Entry(animalType).State = EntityState.Modified;
            _logger.Information("AnimalType {name} updated, Id: {id}", animalType.Name, animalType.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(AnimalType animalType)
        {
            GenericResult result = new GenericResult();
            if (IsValidName(animalType.Name))
            {
                animalType.SerializedName = animalType.Name.ToUpper().Replace(" ", "");
                if (await IsRepeated(animalType.SerializedName) && animalType.Id == 0)// Si estamos repitiendo y no estamos editando
                {
                    //Devolver result con mensaje de error
                    result.AddError(REPEATED_ANIMAL_TYPE_ERROR);
                }
                if (result.Success)
                {
                    if (animalType.Id > 0)
                    {
                        if (!await Update(animalType))
                        {
                            result.AddError(SAVING_ERROR);
                        }
                    }
                    else if (!await Insert(animalType)) result.AddError(SAVING_ERROR);
                }
            }
            else
            {
                result.AddError(INVALID_NAME_ERROR);
            }
            return result;
        }

        public bool IsValidName(string name) { 
            if (name == null) return false;
            if(name.Length <= 0 || name.Length > 35) return false;
            // Chequeo que sean caracteres de a - Z con espacios
            var match = Regex.Match(name, "^[a-zA-Z ]+$");
            if (!match.Success) return false;
            return true;
        }

        public async Task<bool> IsRepeated(string serializedName)
        {
            var existingAnimalTypeCount = await Task.Run(() => _context.AnimalTypes.Count(a => a.SerializedName == serializedName));
            if (existingAnimalTypeCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> HasPetsAssociated(AnimalType animalType)
        {
            var petsFromThisAnimalType = await _context.Pets.
                Include(p => p.AnimalType).
                Where(p => p.AnimalType.SerializedName == animalType.SerializedName).
                ToListAsync();
            if (petsFromThisAnimalType.Count() == 0)
            {
                return false;
            }
            return true;
        }
    }

    [Serializable]
    public class AnimalTypeAlreadyExistsException : Exception
    {
        public AnimalTypeAlreadyExistsException() : base() { }
        public AnimalTypeAlreadyExistsException(string message) : base(message) { }
        public AnimalTypeAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        protected AnimalTypeAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
