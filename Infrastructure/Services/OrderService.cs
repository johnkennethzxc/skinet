using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        // private readonly IGenericRepository<Order> _orderRepo;
        // private readonly IGenericRepository<DeliveryMethod> _dmRepo;
        // private readonly IGenericRepository<Product> _productRepo;
        // private readonly IBasketRepository _basketRepo;
        // public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<DeliveryMethod> dmRepo,
        //     IGenericRepository<Product> productRepo, IBasketRepository basketRepo)
        // {
        //     _basketRepo = basketRepo;
        //     _productRepo = productRepo;
        //     _dmRepo = dmRepo;
        //     _orderRepo = orderRepo;
        // }
        private readonly IPaymentService _paymentService;

        // Using unit of work
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork, 
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // Get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // Get items from product repo
            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                // var productItem = await _productRepo.GetByIdAsync(item.Id);

                //Unit of work
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, 
                    productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            // Get delivery method from repo
            // var deliveryMethod = await _dmRepo.GetByIdAsync(deliveryMethodId);

            //Unit of work
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // Calculate subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // Check if order exists
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder =  await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }

            // Create Order
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, 
                basket.PaymentIntentId);
            
            // Save to Database
            _unitOfWork.Repository<Order>().Add(order);
            
            var result = await _unitOfWork.Complete();

            if (result <= 0) return null;

            // // Delete Basket
            // await _basketRepo.DeleteBasketAsync(basketId);

            // Return Order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}