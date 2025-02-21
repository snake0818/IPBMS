public interface IVariableService
{
  bool StatusSharedFolder { get; set; }
  string SharedFolder_Path { get; set; }
  string LocalFolder_Path { get; set; }
  bool StatusModel { get; set; }
  string Model_URL { get; set; }
}