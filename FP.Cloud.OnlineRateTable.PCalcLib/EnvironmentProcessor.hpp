#pragma once

#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

public interface class IEnvironmentProcessor
{
	void Handle(Shared::EnvironmentInfo^ environment);
};

private ref class EnvironmentProcessor : public IEnvironmentProcessor
{
public:
	EnvironmentProcessor(PCalcFactory^ factory);
	virtual void Handle(Shared::EnvironmentInfo^ environment);

private:
	PCalcFactory^ m_Factory;

};

END_PCALC_LIB_NAMESPACE