<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/IntegerValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/TextValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/Culture.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/DateTimeValue.php';
require_once 'RateTableFileInfo.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\IntegerValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\TextValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\Culture;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\DateTimeValue;


/**
 * Description of newPHPClass
 *
 * @author scharfenberg
 */
class RateTableInfo {

    public $Id;             // integer
    public $Variant;        // string
    public $VersionNumber;  // string
    public $CarrierId;      // integer
    public $CarrierDetails; // integer
    public $ValidFrom;      // DateTime
    public $Culture;        // string
    public $PackageFiles;   // array of RateTableInfo;
    
    
    private function __construct(
            $Id,
            $Variant,
            $VersionNumber,
            $CarrierId,
            $CarrierDetails,
            $ValidFrom,
            $Culture,
            array $PackageFiles) {
        
        $this->Id = new IntegerValue($Id);
        $this->Variant = new TextValue($Variant);
        $this->VersionNumber = new TextValue($VersionNumber);
        $this->CarrierId = new IntegerValue($CarrierId);
        $this->CarrierDetails = new TextValue($CarrierDetails);
        $this->ValidFrom = new DateTimeValue("Y-m-d\TH:i:s", $ValidFrom);
        $this->Culture = new Culture($Culture);
        $this->PackageFiles = $PackageFiles;
    }
    
    static public function createFromStdClass($rateTableInfo) {
        
        $packageFiles = array_map(
                [RateTableFileInfo::fqcn(), 'createFromStdClass'],
                $rateTableInfo->PackageFiles);
            
        $result = new self(
                $rateTableInfo->Id,
                $rateTableInfo->Variant,
                $rateTableInfo->VersionNumber,
                $rateTableInfo->CarrierId,
                $rateTableInfo->CarrierDetails,
                $rateTableInfo->ValidFrom,
                $rateTableInfo->Culture,
                $packageFiles);
        
        return $result;
    }
    
    /**
     * service method that returns the full qualified class name
     */
    static public function fqcn() {
        return __CLASS__;
    }
}
