using Agora.DAOs.KleHierarchy.DTOs;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.Files;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agora.DAOs.KleHierarchy
{
    public class KleService : IKleService
    {
        private readonly IFileService _fileServiceDao;
        private readonly IMapper _mapper;

        public KleService(IFileService fileServiceDao, IMapper mapper)
        {
            _fileServiceDao = fileServiceDao;
            _mapper = mapper;
        }

        public async Task<List<Agora.Models.Models.KleHierarchy>> GetKleHierarchies()
        {
            var dockerEnvironment = Primitives.Logic.Environment.IsRunningInDocker();

            var dockerPath = "/app/wwwroot/api/KleHierarchyList.json";
            var normalPath = "..\\Api\\wwwroot\\api\\KleHierarchyList.json";

            var pathToUse = dockerEnvironment ? dockerPath : normalPath;

            var file = await _fileServiceDao.GetFileFromDisk(pathToUse);

            if (file == null)
            {
                return new List<Agora.Models.Models.KleHierarchy>();
            }

            var fileAsString = System.Text.Encoding.Default.GetString(file);
            var mainKleGroups = JsonSerializer.Deserialize<List<KleMainGroupDto>>(fileAsString);

            var mainKleGroupsModel = mainKleGroups.Select(kleGroup => _mapper.Map<Agora.Models.Models.KleHierarchy>(kleGroup));

            return mainKleGroupsModel.ToList();
        }
    }
}