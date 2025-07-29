//using FoodService.Application.Exceptions;
//using FoodService.Application.Interfaces;
//using FoodService.Application.UseCases.Queries.Dish;
//using FoodService.Domain.Interfaces;

//namespace FoodService.Application.UseCases.QueryHandlers.Dish
//{
//    public class GetDishImageHandler : IQueryHandler<GetDishImageQuery, Stream>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IImageService _imageService;

//        public GetDishImageHandler(IUnitOfWork unitOfWork, IImageService imageService)
//        {
//            _unitOfWork = unitOfWork;
//            _imageService = imageService;
//        }

//        public async Task<Stream> Handle(GetDishImageQuery request, CancellationToken cancellationToken)
//        {
//            var dish = await _unitOfWork.DishRepository.GetByIdAsync(request.DishId);

//            if (dish == null)
//            {
//                throw new NotFound("Dish not found");
//            }

//            var imagePath = dish.ImageUrl;

//            if (imagePath == null)
//            {
//                throw new NotFound("Dish doesn't have image");
//            }

//            var imageStream = await _imageService.DownloadImageAsync(imagePath);

//            if (imageStream == null)
//            {
//                throw new NotFound("Failed getting image");
//            }

//            return imageStream;
//        }
//    }
//}
