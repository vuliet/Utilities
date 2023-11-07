using Utilities.Models;
using Utilities.Utilities;

Console.WriteLine("Utilities !");

var originObj = new AppModels()
{
    Id = 1,
    Name = "Thai"
};

var newObj = ObjectUtils.DeepCopy(originObj);

var listNewObj = new List<AppModels>();

listNewObj.Add(originObj);

listNewObj.First().Name = "Heo";

if (ReferenceEquals(originObj, listNewObj.First()))
    Console.WriteLine("obj1 và obj2 tham chieu den cung mot doi tuong.");
else
    Console.WriteLine("obj1 và obj2 tham chieu den doi tuong khác nhau.");

Console.ReadLine();


