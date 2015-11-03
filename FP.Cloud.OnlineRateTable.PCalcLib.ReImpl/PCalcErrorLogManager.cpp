//## begin module%1.8%.codegen_version preserve=yes
//   Read the documentation to learn more about C++ code generator
//   versioning.
//## end module%1.8%.codegen_version

//## begin module%4EB0FC380116.cm preserve=no
//	VERSION-INFO
//	$Revision$
//	$Date$
//	$Author$
//## end module%4EB0FC380116.cm

//## begin module%4EB0FC380116.cp preserve=no
//## end module%4EB0FC380116.cp

//## Module: PCalcErrorLogManager%4EB0FC380116; Package body
//## Subsystem: ProductCalculation::src%4D10694701E9
//## Source file: C:\projekte\FMLibs\ProductCalculation\src\PCalcErrorLogManager.cpp

//## begin module%4EB0FC380116.additionalIncludes preserve=no
//## end module%4EB0FC380116.additionalIncludes

//## begin module%4EB0FC380116.includes preserve=yes
#include "Base/misc/assert_m.h"
#include "Base/DateTime/WallClock.hpp"
#include "ProductCalculation/FactoryPCalc.hpp"
#include "ProductCalculation/ProdCalcException.hpp"
#include "ProductCalculation/src/PCalcErrorLogImpl.hpp"
//## end module%4EB0FC380116.includes

// PCalcErrorLogManager
#include "ProductCalculation/src/PCalcErrorLogManager.hpp"
//## begin module%4EB0FC380116.declarations preserve=no
//## end module%4EB0FC380116.declarations

//## begin module%4EB0FC380116.additionalDeclarations preserve=yes
//## end module%4EB0FC380116.additionalDeclarations


namespace ProductCalculation {
    //## begin ProductCalculation%4787400F01A3.initialDeclarations preserve=yes
    //## end ProductCalculation%4787400F01A3.initialDeclarations

    // Class ProductCalculation::PCalcErrorLogManager 


    PCalcErrorLogManager::PCalcErrorLogManager()
      //## begin PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.hasinit preserve=no
      //## end PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.hasinit
      //## begin PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.initialization preserve=yes
	  : m_CurrentLogEntry()
	  , m_AccessMutex()
	  , m_pErrorLog()
      , m_IsTransaction(false)
      //## end PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.initialization
    {
      //## begin ProductCalculation::PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.body preserve=yes
      //## end ProductCalculation::PCalcErrorLogManager::PCalcErrorLogManager%4EAF925602D0_const.body
    }


    PCalcErrorLogManager::~PCalcErrorLogManager()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::~PCalcErrorLogManager%4EAF925602D0_dest.body preserve=yes
      //## end ProductCalculation::PCalcErrorLogManager::~PCalcErrorLogManager%4EAF925602D0_dest.body
    }



    //## Other Operations (implementation)
    EXTENDED_ERROR_CODE PCalcErrorLogManager::Begin ()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::Begin%4EAF93AC031E.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::Begin%4EAF93AC031E.body
    }

    EXTENDED_ERROR_CODE PCalcErrorLogManager::Commit ()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::Commit%4EAF93AC032D.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::Commit%4EAF93AC032D.body
    }

    EXTENDED_ERROR_CODE PCalcErrorLogManager::Rollback ()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::Rollback%4EAF93AC033D.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::Rollback%4EAF93AC033D.body
    }

    bool PCalcErrorLogManager::Create ()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::Create%4EAF963E0200.body preserve=yes
		return true;
      //## end ProductCalculation::PCalcErrorLogManager::Create%4EAF963E0200.body
    }

    EXTENDED_ERROR_CODE PCalcErrorLogManager::Initialize (IPCalcErrorLogPtr errorLogImpl)
    {
      //## begin ProductCalculation::PCalcErrorLogManager::Initialize%4EAFAFA10085.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::Initialize%4EAFAFA10085.body
    }

    EXTENDED_ERROR_CODE PCalcErrorLogManager::GetLogEntries (LogEntryList& entries)
    {
      //## begin ProductCalculation::PCalcErrorLogManager::GetLogEntries%4EAFE3970194.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::GetLogEntries%4EAFE3970194.body
    }

    EXTENDED_ERROR_CODE PCalcErrorLogManager::SaveLogEntries ()
    {
      //## begin ProductCalculation::PCalcErrorLogManager::SaveLogEntries%4F3A7CB402F7.body preserve=yes
		return EXTENDED_ERROR_CODE();
      //## end ProductCalculation::PCalcErrorLogManager::SaveLogEntries%4F3A7CB402F7.body
    }

    // Additional Declarations
      //## begin ProductCalculation::PCalcErrorLogManager%4EAF925602D0.declarations preserve=yes
      //## end ProductCalculation::PCalcErrorLogManager%4EAF925602D0.declarations

} // namespace ProductCalculation

//## begin module%4EB0FC380116.epilog preserve=yes
//## end module%4EB0FC380116.epilog
