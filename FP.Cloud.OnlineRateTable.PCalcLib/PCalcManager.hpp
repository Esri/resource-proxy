#pragma once

#include "PCalcLib.hpp"
#include "Base/misc/std_def.h"

BEGIN_PCALC_LIB_NAMESPACE

ref class PCalcFactory;

private interface class IPCalcManager
{
	void Create();
	void Initialize();
	void LoadPawn(System::String^ file);
	void LoadProductTable(System::String^ file);
	void Unload();
	void CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	void CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	void Calculate([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	void CalculateBack([System::Runtime::InteropServices::Out] INT32 %rNextAction);
};

private ref class PCalcManager : public IPCalcManager
{
public:
	PCalcManager(PCalcFactory^ factory);

	virtual void Create();
	virtual void Initialize();
	virtual void LoadPawn(System::String^ file);
	virtual void LoadProductTable(System::String^ file);
	virtual void Unload();
	virtual void CalculateStart([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	virtual void CalculateNext([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	virtual void Calculate([System::Runtime::InteropServices::Out] INT32 %rNextAction);
	virtual void CalculateBack([System::Runtime::InteropServices::Out] INT32 %rNextAction);

private:
	PCalcFactory^ m_Factory;
};

END_PCALC_LIB_NAMESPACE