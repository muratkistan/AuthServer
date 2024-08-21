using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _repository;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _repository = genericRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<TDto>> AddAsync(TDto dto)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);

            await _repository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            return Response<TDto>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var productDtos = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await _repository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Success(productDtos, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null)
            {
                return Response<TDto>.Fail("Id not found", 404, true);
            }
            return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _repository.GetByIdAsync(id);

            if (isExistEntity is null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            _repository.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> Update(TDto dto, int id)
        {
            var isExistEntity = await _repository.GetByIdAsync(id);

            if (isExistEntity is null)
            {
                return Response<NoDataDto>.Fail("Id not found", 404, true);
            }

            var updatedEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            _repository.Update(updatedEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _repository.Where(predicate);

            var listDto = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync());

            return Response<IEnumerable<TDto>>.Success(listDto, 200);
        }
    }
}