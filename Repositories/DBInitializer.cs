using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public static class DBInitializer
    {
        public static async Task Initialize(
            SWD392_G3DBcontext context,
            UserManager<User> userManager)
        {
            #region Seed Roles
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "ADMIN", NormalizedName = "ADMIN" },
                    new Role { Name = "USER", NormalizedName = "USER" },
                    new Role { Name = "STAFF", NormalizedName = "STAFF" },
                    new Role { Name = "MANAGER", NormalizedName = "MANAGER" }
                };

                await context.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
            #endregion

            #region Seed Services
            if (!context.Services.Any())
            {
                var services = new List<Service>{
                    new Service
                    {
                        Name = "Tắm gội toàn thân",
                        Description = "Tắm gội nhẹ dịu dành cho chó/mèo, giúp sạch sẽ và thơm tho.",
                        Price = 150,
                        Duration = 40,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Cắt tỉa lông tạo kiểu",
                        Description = "Cắt lông thẩm mỹ theo yêu cầu, phong cách dễ thương/chất chơi.",
                        Price = 250,
                        Duration = 60,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Vệ sinh tai",
                        Description = "Làm sạch tai và kiểm tra các dấu hiệu viêm nhiễm.",
                        Price = 80,
                        Duration = 15,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Cạo lông sát mùa hè",
                        Description = "Cạo lông ngắn giúp thú cưng mát mẻ hơn trong mùa nóng.",
                        Price = 180,
                        Duration = 30,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Cắt móng",
                        Description = "Cắt móng gọn gàng, tránh thú cưng tự làm đau mình.",
                        Price = 50,
                        Duration = 10,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Massage thư giãn",
                        Description = "Giúp thú cưng thư giãn, lưu thông máu tốt hơn.",
                        Price = 120,
                        Duration = 25,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Tẩy giun định kỳ",
                        Description = "Tẩy giun an toàn cho thú cưng, khuyến nghị mỗi 3 tháng.",
                        Price = 90,
                        Duration = 20,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Tiêm phòng bệnh dại",
                        Description = "Phòng bệnh dại bắt buộc với chó, áp dụng từ 3 tháng tuổi.",
                        Price = 200,
                        Duration = 10,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Tiêm phòng 7 bệnh",
                        Description = "Tiêm phòng tổng hợp phòng ngừa nhiều bệnh nguy hiểm.",
                        Price = 350,
                        Duration = 15,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Chăm sóc hậu phẫu",
                        Description = "Chăm sóc thú cưng sau khi triệt sản hoặc phẫu thuật nhỏ.",
                        Price = 300,
                        Duration = 60,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Spa xả stress",
                        Description = "Gói combo massage, tắm thơm, sấy khô, chơi đùa.",
                        Price = 400,
                        Duration = 90,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Xét nghiệm máu cơ bản",
                        Description = "Kiểm tra tình trạng máu, gan, thận, phát hiện bệnh sớm.",
                        Price = 320,
                        Duration = 30,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Khám sức khỏe định kỳ",
                        Description = "Khám tổng quát, tư vấn dinh dưỡng, kiểm tra các chỉ số.",
                        Price = 250,
                        Duration = 45,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Gửi thú cưng qua đêm",
                        Description = "Dịch vụ lưu trú 5 sao, có camera, ăn uống đúng giờ.",
                        Price = 500,
                        Duration = 720,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Huấn luyện cơ bản",
                        Description = "Dạy thú cưng ngồi, nằm, bắt tay, đi vệ sinh đúng chỗ.",
                        Price = 600,
                        Duration = 90,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Tắm khử mùi siêu sạch",
                        Description = "Tắm đặc biệt khử mùi cho chó/mèo nặng mùi.",
                        Price = 180,
                        Duration = 40,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Gắn chip định danh",
                        Description = "Chip theo dõi + nhận diện chủ, chống thất lạc thú cưng.",
                        Price = 700,
                        Duration = 15,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Xét nghiệm ký sinh trùng",
                        Description = "Phát hiện ký sinh trùng đường ruột, ngoài da,...",
                        Price = 300,
                        Duration = 35,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Chăm sóc răng miệng",
                        Description = "Vệ sinh răng, loại bỏ cao răng, khử mùi hôi miệng.",
                        Price = 220,
                        Duration = 30,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new Service
                    {
                        Name = "Triệt sản chó/mèo",
                        Description = "Phẫu thuật triệt sản an toàn, hồi phục nhanh.",
                        Price = 1200,
                        Duration = 180,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    }
                };
                await context.AddRangeAsync(services);
                await context.SaveChangesAsync();

            }
            #endregion
            #region Seed Users
            if (!context.Users.Any())
            {
                // Seed Admin user
                var admin = new User
                {
                    UserName = "hctrung2k4",
                    Email = "hctrung2k4@gmail.com",
                    FirstName = "Hoang Chi",
                    LastName = "Trung",
                    Gender = "Male",
                    PhoneNumber = "0339381305",
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };
                await CreateUserAsync(userManager, admin, "string", "ADMIN");

                // Seed Staff user
                var staff = new User
                {
                    UserName = "staff",
                    Email = "staff@staff.com",
                    FirstName = "Staff",
                    LastName = "User",
                    Gender = "Female", // hoặc giá trị phù hợp
                    PhoneNumber = "0123456789",
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };
                await CreateUserAsync(userManager, staff, "string", "USER");    

                // Seed một số khách hàng (customers)
                var customers = new List<User>
                {
                    new User
                    {
                        UserName = "staf",
                        Email = "stafMaster@gmail.com",
                        FirstName = "Staff",
                        LastName = "Master",
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new User
                    {
                        UserName = "peter",
                        Email = "peter@fpt.edu.vn",
                        FirstName = "Peter",
                        LastName = "Hiller",
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new User
                    {
                        UserName = "kaka",
                        Email = "kaka@fpt.edu.vn",
                        FirstName = "Kaka",
                        LastName = "User",
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new User
                    {
                        UserName = "musk",
                        Email = "musk@fpt.edu.vn",
                        FirstName = "Musk",
                        LastName = "Mon",
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new User
                    {
                        UserName = "gate",
                        Email = "gate@fpt.edu.vn",
                        FirstName = "Gate",
                        LastName = "Gill",
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    },
                    new User
                    {
                        UserName = "Trung",
                        Email = "trunghcse181597@fpt.edu.vn",
                        FirstName = "Trung",
                        LastName = "HC",
                        EmailConfirmed = true,
                        Gender = "Male",
                        PhoneNumber = "0123456789",
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    }
                };

                foreach (var customer in customers)
                {
                    // Bạn có thể tùy chọn gán role mặc định cho khách hàng nếu cần, ví dụ: "USER"
                    await CreateUserAsync(userManager, customer, "string", "USER");
                }
            }
            #endregion

            // Cập nhật SecurityStamp cho các user nếu chưa có
            var allUsers = await context.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    Console.WriteLine($"Security stamp updated for user {user.UserName}");
                }
            }
        }

        private static async Task CreateUserAsync(UserManager<User> userManager, User user, string password, string role)
        {
            var userExist = await userManager.FindByEmailAsync(user.Email!);
            if (userExist == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    // Log lỗi chi tiết
                    var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Error creating user {user.Email}: {errorMsg}");
                }
            }
            else
            {
                Console.WriteLine($"User {user.Email} already exists.");
            }
        }
    }
}
