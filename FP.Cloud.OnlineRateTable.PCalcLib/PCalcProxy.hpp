#pragma once

#include "PCalcLib.hpp"
#include "IPCalcProxy.hpp"

namespace ProductCalculation
{
	struct ActionResultType;
}

BEGIN_PCALC_LIB_NAMESPACE

ref	class CalculationResultProcessorProxy;
ref class ActionResultProcessor;
ref class EnvironmentProcessor;
ref class ProductDescriptionMapper;
ref class PCalcManager;
ref class PCalcFactory;

private ref class PCalcProxy : System::MarshalByRefObject, public IPCalcProxy
{
public:
	PCalcProxy();
	~PCalcProxy();

	virtual Shared::PCalcResultInfo^ Start(Shared::EnvironmentInfo^ environment, Shared::WeightInfo^ weight);
	virtual Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product, Shared::ActionResultInfo^ actionResult);
	virtual Shared::PCalcResultInfo^ Calculate(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);
	virtual Shared::PCalcResultInfo^ Back(Shared::EnvironmentInfo^ environment, Shared::ProductDescriptionInfo^ product);

	virtual void Init(System::String^ amxPath, System::String^ tablePath);

protected:
	!PCalcProxy();

private:
	static System::Object^ m_SyncLock = gcnew System::Object();
	PCalcFactory^ m_Factory;
	PCalcManager^ m_Manager;
	CalculationResultProcessorProxy^ m_CalculationResultProcessor;
	ActionResultProcessor^ m_ActionResultProcessor;
	EnvironmentProcessor^ m_EnvironmentProcessor;
	ProductDescriptionMapper^ m_ProductDescriptionMapper;
};

END_PCALC_LIB_NAMESPACE




