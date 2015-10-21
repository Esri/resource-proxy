#pragma once

#include "PCalcLib.hpp"
#include "ExtendedErrorCode.hpp"
#include "IPCalcProxy.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

private ref class PCalcProxy : System::MarshalByRefObject, public IPCalcProxy
	{
	public:
		PCalcProxy();
		~PCalcProxy();

		void Create();
		void LoadPawn(System::String^ file);
		void LoadProductTable(System::String^ file);

		virtual PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, WeightInfo^ weight);
		virtual PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
		
		void Unload();

	protected:
		!PCalcProxy();
		void Initialize(EnvironmentInfo^ environment);
		void Initialize();
		void CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction);
		void CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction);
		PCalcResultInfo^ ProcessResult(INT32 nextAction);
		void SetEnvironment(EnvironmentInfo^ environment);
		void SetWeight(WeightInfo^ weight);

	private:
		PCalcFactory^ m_Factory;

	};

END_PCALC_LIB_NAMESPACE