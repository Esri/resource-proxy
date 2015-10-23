#pragma once

#include "PCalcFactory.hpp"
#include "PCalcLib.hpp"

BEGIN_PCALC_LIB_NAMESPACE

private ref class PCalcManager
{
public:
	PCalcManager(PCalcFactory^ factory);

	void Create();
	void Initialize();
	void LoadPawn(System::String^ file);
	void LoadProductTable(System::String^ file);
	void Unload();
	void CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	void CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	void WeightChanged();

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE