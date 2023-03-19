//using DuyProject.API.Configurations;
//using DuyProject.API.Models;
//using MongoDB.Driver;

//namespace DuyProject.API.Helpers
//{
//    public static class DataVerifyHelper
//    {
//        private static readonly IMongoDatabase Database = new MongoClient().GetDatabase(AppSettings.DbName);
//        private static readonly IMongoCollection<Cause> CauseCollection = Database.GetCollection<Cause>(nameof(Cause));
//        private static readonly IMongoCollection<Drug> DrugCollection = Database.GetCollection<Drug>(nameof(Drug));

//        public static bool DrugVerifyHelper(Pharmacy entity)
//        {
//            return entity.DrugIds.Select(drugId => DrugCollection.AsQueryable().Any(x => x.Id == drugId)).Any(isDrugIdValid => !isDrugIdValid);
//        }
//    }
//}
