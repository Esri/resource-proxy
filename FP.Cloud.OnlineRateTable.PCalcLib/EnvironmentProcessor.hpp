#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

private ref class EnvironmentProcessor
{
public:
	EnvironmentProcessor(PCalcFactory^ factory);
	void Handle(Shared::EnvironmentInfo^ environment);

private:
	PCalcFactory^ m_Factory;

};

END_PCALC_LIB_NAMESPACE