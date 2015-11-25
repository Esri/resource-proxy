<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/IntegerValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/IntegerSelectionValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/TextValue.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\IntegerValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\IntegerSelectionValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\TextValue;


/**
 * Description of newPHPClass1
 *
 * @author scharfenberg
 */
class RateTableFileInfo {

    const FILETYPE_UNDEFINED = 0;
    const FILETYPE_RATETABLE = 1;
    const FILETYPE_PAWNCODE = 2;
    const FILETYPE_ZIP2ZONE = 3;
    const FILETYPE_META = 4;
    
    
    public $Id;         // integer
    public $FileName;   // string
    public $FileType;   // integer
    public $FileData;   // TODO
    public $Checksum;   // integer
    
    
    private function __construct(
            $Id,
            $FileName,
            $FileType,
            array $FileData,
            $Checksum) {
        
        $this->Id = new IntegerValue($Id);
        $this->FileName = new TextValue($FileName);
        $this->FileType
                = new IntegerSelectionValue(self::validFileTypes(), $FileType);
        $this->FileData = $FileData;
        $this->Checksum = new IntegerValue($Checksum);
    }
    
    static private function validFileTypes() {
        
        static $validFileTypes = null;
        if(!isset($validFileTypes)) {
            $refl = new \ReflectionClass(__CLASS__);
            $constants = $refl->getConstants();
            $validFileTypes = array_values($constants);
        }
        
        return $validFileTypes;
    }
    
    static public function createFromStdClass($rateTableFileInfo) {
        
        $result = new self(
                $rateTableFileInfo->Id,
                $rateTableFileInfo->FileName,
                $rateTableFileInfo->FileType,
                $rateTableFileInfo->FileData,
                $rateTableFileInfo->Checksum );
        
        return $result;
    }
    
    /**
     * service method that returns the full qualified class name
     */
    static public function fqcn() {
        return __CLASS__;
    }
}
