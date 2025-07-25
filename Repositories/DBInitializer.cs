﻿using Azure.Core;
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

            #region Seeed Memberships
            if (!context.Memberships.Any())
            {
                var memberships = new List<Membership>
                {
                   new Membership
                    {
                        Name = "Basic",
                        Price = 0,
                        Description = "Gói miễn phí cơ bản dành cho khách mới.",
                        DurationInDays = 0,
                        DiscountPercentage = 0,
                        ImageUrl = "https://example.com/membership-basic.png",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Membership
                    {
                        Name = "Bronze",
                        Price = 99000,
                        Description = "Giảm giá nhẹ nhàng cho các đơn hàng nhỏ.",
                        DurationInDays = 30,
                        DiscountPercentage = 5,
                        ImageUrl = "https://example.com/membership-bronze.png",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Membership
                    {
                        Name = "Silver",
                        Price = 199000,
                        Description = "Ưu đãi ổn định cho khách thường xuyên.",
                        DurationInDays = 60,
                        DiscountPercentage = 10,
                        ImageUrl = "https://example.com/membership-silver.png",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Membership
                    {
                        Name = "Gold",
                        Price = 299000,
                        Description = "Giảm mạnh tay cho đơn hàng lớn và combo.",
                        DurationInDays = 90,
                        DiscountPercentage = 15,
                        ImageUrl = "https://example.com/membership-gold.png",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Membership
                    {
                        Name = "Platinum",
                        Price = 499000,
                        Description = "Tối đa hóa lợi ích cho khách VIP.",
                        DurationInDays = 180,
                        DiscountPercentage = 20,
                        ImageUrl = "https://example.com/membership-platinum.png",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await context.AddRangeAsync(memberships);
                await context.SaveChangesAsync();
            }
            #endregion
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
            #region seed Staffs
            if (!context.Staffs.Any() && !context.StaffSchedules.Any())
            {
                // 1. Seed User
                var users = new List<User>
{
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "An",
                        LastName = "Nguyễn",
                        Email = "an.nguyen@example.com",
                        UserName = "an.nguyen",
                        PhoneNumber = "0911111111",
                        Gender = "Nam",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Bình",
                        LastName = "Trần",
                        Email = "binh.tran@example.com",
                        UserName = "binh.tran",
                        PhoneNumber = "0922222222",
                        Gender = "Nữ",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Cường",
                        LastName = "Lê",
                        Email = "cuong.le@example.com",
                        UserName = "cuong.le",
                        PhoneNumber = "0933333333",
                        Gender = "Nam",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Duyên",
                        LastName = "Phạm",
                        Email = "duyen.pham@example.com",
                        UserName = "duyen.pham",
                        PhoneNumber = "0944444444",
                        Gender = "Nữ",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Em",
                        LastName = "Đỗ",
                        Email = "em.do@example.com",
                        UserName = "em.do",
                        PhoneNumber = "0955555555",
                        Gender = "Nam",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                //await context.Users.AddRangeAsync(users);
                //await context.SaveChangesAsync(); // ✅ Lưu Users trước
                foreach (var staff in users)
                    await CreateUserAsync(userManager, staff, "string", "Staff");
                // 2. Seed Staff dựa theo User
                var staffs = new List<Staff>();
                foreach (var user in users)
                {
                    staffs.Add(new Staff
                    {
                        Id = user.Id,
                        User = user,
                        Salary = new Random().Next(800, 1200),
                        HireDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await context.Staffs.AddRangeAsync(staffs);
                await context.SaveChangesAsync(); // ✅ Lưu Staffs

                // 3. Seed StaffSchedules cho mỗi staff
                var schedules = new List<StaffSchedule>();
                foreach (var staff in staffs)
                {
                    for (int day = 1; day <= 6; day++) // Thứ 2 đến thứ 7
                    {
                        // Ca sáng
                        schedules.Add(new StaffSchedule
                        {
                            StaffScheduleId = Guid.NewGuid(),
                            StaffId = staff.Id,
                            DayOfWeek = (DayOfWeek)day,
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(12, 0, 0),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });

                        // Ca chiều
                        schedules.Add(new StaffSchedule
                        {
                            StaffScheduleId = Guid.NewGuid(),
                            StaffId = staff.Id,
                            DayOfWeek = (DayOfWeek)day,
                            StartTime = new TimeSpan(13, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
                await context.StaffSchedules.AddRangeAsync(schedules);
                await context.SaveChangesAsync(); // ✅ Lưu lịch
            }
            #endregion
            #region Seed Services
            if (!context.Services.Any())
            {
                var now = DateTime.UtcNow.AddHours(7);
                var defaultImg = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQzDPMHdH3nHA9rsDDo3TfQKmDJCvlcxH209A&s";

                var services = new List<Service>
                            {
                                new()
                                {
                                    Name = "Tắm gội toàn thân",
                                    Description = "Tắm gội nhẹ dịu dành cho chó/mèo, giúp sạch sẽ và thơm tho.",
                                    Price = 150000,
                                    Duration = 40,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Cắt tỉa lông tạo kiểu",
                                    Description = "Cắt lông thẩm mỹ theo yêu cầu, phong cách dễ thương/chất chơi.",
                                    Price = 250000,
                                    Duration = 60,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Vệ sinh tai",
                                    Description = "Làm sạch tai và kiểm tra các dấu hiệu viêm nhiễm.",
                                    Price = 80000,
                                    Duration = 15,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Cạo lông sát mùa hè",
                                    Description = "Cạo lông ngắn giúp thú cưng mát mẻ hơn trong mùa nóng.",
                                    Price = 180000,
                                    Duration = 30,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Cắt móng",
                                    Description = "Cắt móng gọn gàng, tránh thú cưng tự làm đau mình.",
                                    Price = 50000,
                                    Duration = 10,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Massage thư giãn",
                                    Description = "Giúp thú cưng thư giãn, lưu thông máu tốt hơn.",
                                    Price = 120000,
                                    Duration = 25,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Tẩy giun định kỳ",
                                    Description = "Tẩy giun an toàn cho thú cưng, khuyến nghị mỗi 3 tháng.",
                                    Price = 90000,
                                    Duration = 20,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Tiêm phòng bệnh dại",
                                    Description = "Phòng bệnh dại bắt buộc với chó, áp dụng từ 3 tháng tuổi.",
                                    Price = 200000,
                                    Duration = 10,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Tiêm phòng 7 bệnh",
                                    Description = "Tiêm phòng tổng hợp phòng ngừa nhiều bệnh nguy hiểm.",
                                    Price = 350000,
                                    Duration = 15,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Chăm sóc hậu phẫu",
                                    Description = "Chăm sóc thú cưng sau khi triệt sản hoặc phẫu thuật nhỏ.",
                                    Price = 300000,
                                    Duration = 60,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Spa xả stress",
                                    Description = "Gói combo massage, tắm thơm, sấy khô, chơi đùa.",
                                    Price = 400000,
                                    Duration = 90,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Xét nghiệm máu cơ bản",
                                    Description = "Kiểm tra tình trạng máu, gan, thận, phát hiện bệnh sớm.",
                                    Price = 320000,
                                    Duration = 30,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Khám sức khỏe định kỳ",
                                    Description = "Khám tổng quát, tư vấn dinh dưỡng, kiểm tra các chỉ số.",
                                    Price = 250000,
                                    Duration = 45,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Gửi thú cưng qua đêm",
                                    Description = "Dịch vụ lưu trú 5 sao, có camera, ăn uống đúng giờ.",
                                    Price = 500000,
                                    Duration = 720,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Huấn luyện cơ bản",
                                    Description = "Dạy thú cưng ngồi, nằm, bắt tay, đi vệ sinh đúng chỗ.",
                                    Price = 600000,
                                    Duration = 90,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Tắm khử mùi siêu sạch",
                                    Description = "Tắm đặc biệt khử mùi cho chó/mèo nặng mùi.",
                                    Price = 180000,
                                    Duration = 40,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Gắn chip định danh",
                                    Description = "Chip theo dõi + nhận diện chủ, chống thất lạc thú cưng.",
                                    Price = 700000,
                                    Duration = 15,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Xét nghiệm ký sinh trùng",
                                    Description = "Phát hiện ký sinh trùng đường ruột, ngoài da,...",
                                    Price = 300000,
                                    Duration = 35,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Chăm sóc răng miệng",
                                    Description = "Vệ sinh răng, loại bỏ cao răng, khử mùi hôi miệng.",
                                    Price = 220000,
                                    Duration = 30,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                                },
                                new()
                                {
                                    Name = "Triệt sản chó/mèo",
                                    Description = "Phẫu thuật triệt sản an toàn, hồi phục nhanh.",
                                    Price = 1200000,
                                    Duration = 180,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    imgURL = defaultImg
                            }
                                        }
            ;
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
                    await CreateUserAsync(userManager, customer, "string", "USER");
            }
            #endregion
            #region Seed Manager
            if (!context.Managers.Any())
            {
                // Seed Manager
                var manager = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Manager",
                    LastName = "And Admin",
                    Email = "managerAdmin@example.com",
                    UserName = "ManagerAdmin",
                    PhoneNumber = "0961111111",
                    Gender = "Nam",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await CreateUserAsync(userManager, manager, "string", "MANAGER");
                await userManager.AddToRoleAsync(manager, "Admin");
                var managerr = new Manager
                {
                    UserId = manager.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                }; 
                await context.Managers.AddAsync(managerr);
                await context.SaveChangesAsync();


            }
            #endregion Seed Managers
            #region Seed Customers
            if (!context.Customers.Any())
            {
                var customerUsers = new List<User>
                {
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Hạnh",
                        LastName = "Phạm",
                        Email = "hanh.pham@example.com",
                        UserName = "hanh.pham",
                        PhoneNumber = "0961111111",
                        Gender = "Nữ",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Khánh",
                        LastName = "Ngô",
                        Email = "khanh.ngo@example.com",
                        UserName = "khanh.ngo",
                        PhoneNumber = "0972222222",
                        Gender = "Nam",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Linh",
                        LastName = "Đinh",
                        Email = "linh.dinh@example.com",
                        UserName = "linh.dinh",
                        PhoneNumber = "0983333333",
                        Gender = "Nữ",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Mạnh",
                        LastName = "Trần",
                        Email = "manh.tran@example.com",
                        UserName = "manh.tran",
                        PhoneNumber = "0994444444",
                        Gender = "Nam",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Nhi",
                        LastName = "Hoàng",
                        Email = "nhi.hoang@example.com",
                        UserName = "nhi.hoang",
                        PhoneNumber = "0915555555",
                        Gender = "Nữ",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach(var cus in customerUsers)
                await CreateUserAsync(userManager, cus, "string", "User");

                // Seed Customer
                var customers = customerUsers.Select(u => new Customer
                {
                    UserId = u.Id,
                    User = u,
                    Address = $"Số {new Random().Next(1, 100)} Đường Fake Street, TP.HCM",
                    imgURL = "https://i.pravatar.cc/150?u=" + u.Email,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync(); // ✅ Lưu Customers
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
