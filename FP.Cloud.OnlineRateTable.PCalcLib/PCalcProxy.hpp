#pragma once

#include "PCalcLib.hpp"
#include "ExtendedErrorCode.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE

using namespace FP::Cloud::OnlineRateTable::Common::ProductCalculation;

public ref class PCalcProxy : System::MarshalByRefObject
	{
	public:
		PCalcProxy();
		~PCalcProxy();

		ExtendedErrorCode^ Create();
		ExtendedErrorCode^ LoadPawn(System::String^ file);
		ExtendedErrorCode^ LoadProductTable(System::String^ file);	

		PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, WeightInfo^ weight);
		PCalcResultInfo^ Calculate(EnvironmentInfo^ environment, ProductDescriptionInfo^ product, ActionResultInfo^ actionResult);
		
		void Unload();

	protected:
		!PCalcProxy();
		ExtendedErrorCode^ Initialize(EnvironmentInfo^ environment);
		ExtendedErrorCode^ Initialize();
		ExtendedErrorCode^ CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction);
		ExtendedErrorCode^ CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction);
		PCalcResultInfo^ ProcessResult(INT32 nextAction);
		void SetEnvironment(EnvironmentInfo^ environment);
		void SetWeight(WeightInfo^ weight);

	private:
		PCalcFactory^ m_Factory;

	};

END_PCALC_LIB_NAMESPACE