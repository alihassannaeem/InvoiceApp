using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Invoice.Application.Common.Mappings
{
    public interface IMapTo<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(T));
    }
}


