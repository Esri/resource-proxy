#pragma once

#include "PCalcLib.hpp"
#include "ProductCalculation/IProductDescParameter.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

public interface class IActionResultProcessor
{
	void Handle(Shared::ActionResultInfo^ actionResult);
};

private ref class ActionResultProcessor : public IActionResultProcessor
{
public:
	ActionResultProcessor(PCalcFactory^ factory);
	virtual void Handle(Shared::ActionResultInfo^ actionResult);

private:
	void SetActionResult(PT::IProductDescParameterPtr &parameter, Shared::ActionResultInfo^ actionResult);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE