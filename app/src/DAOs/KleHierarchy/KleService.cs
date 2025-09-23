using AutoMapper;
using BallerupKommune.DAOs.KleHierarchy.DTOs;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.KleHierarchy
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

        public async Task<List<BallerupKommune.Models.Models.KleHierarchy>> GetKleHierarchies()
        {
            var dockerEnvironment = Primitives.Logic.Environment.IsRunningInDocker();

            var dockerPath = "/app/wwwroot/api/KleHierarchyList.json";
            var normalPath = "..\\Api\\wwwroot\\api\\KleHierarchyList.json";

            var pathToUse = dockerEnvironment ? dockerPath : normalPath;

            var file = await _fileServiceDao.GetFileFromDisk(pathToUse);

            if (file == null)
            {
                return new List<BallerupKommune.Models.Models.KleHierarchy>();
            }

            var fileAsString = System.Text.Encoding.Default.GetString(file);
            var mainKleGroups = JsonSerializer.Deserialize<List<KleMainGroupDto>>(fileAsString);

            var mainKleGroupsModel = mainKleGroups.Select(kleGroup => _mapper.Map<BallerupKommune.Models.Models.KleHierarchy>(kleGroup));

            return mainKleGroupsModel.ToList();
        }
    }
}