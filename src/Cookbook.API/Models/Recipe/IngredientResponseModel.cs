namespace Cookbook.API.Models.Recipe
{
    public class IngredientResponseModel
    {
        public int Id { get; set; }

        public int Amount { get; set; }

        public string MeasurementType { get; set; }

        public string Name { get; set; }

        public int Position { get; set; }
    }
}
