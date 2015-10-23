#pragma once

#include "PCalcLib.hpp"
#include "PCalcFactory.hpp"

BEGIN_PCALC_LIB_NAMESPACE
USING_PRODUCTCALCULATION_NAMESPACE

private ref class EnvironmentProcessor
{
public:
	EnvironmentProcessor(PCalcFactory^ factory);
	void Handle(EnvironmentInfo^ environment);

private:
	PCalcFactory^ m_Factory;

};

END_PCALC_LIB_NAMESPACE