using Common;

namespace dCom.ViewModel
{
    internal abstract class AnalogBase : BasePointItem, IAnalogPoint 
	{
		private double eguValue;

		public AnalogBase(IConfigItem c, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration, int i)
			: base(c, processingManager, stateUpdater, configuration, i)
		{
		}

		public double EguValue
		{
			get
			{
				return eguValue;
			}

			set
			{
				eguValue = value;
				OnPropertyChanged("DisplayValue");
			}
		}

		public override string DisplayValue
		{
			get
			{
				return EguValue.ToString();
			}
		}
        public override ushort RawValue
        {
            get => base.RawValue;
            set
            {
                base.RawValue = value;

                var converter = new ProcessingModule.EGUConverter();

               
                EguValue = value;
            }
        }

        protected override bool WriteCommand_CanExecute(object obj)
        {
            return false;
        }
    }
}
