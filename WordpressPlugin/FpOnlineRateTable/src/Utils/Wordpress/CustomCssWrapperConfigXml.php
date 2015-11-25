<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once 'ICustomCssWrapperConfig.php';


/**
 * Description of CustomCssWrapperConfigXml
 *
 * @author scharfenberg
 */
class CustomCssWrapperConfigXml implements ICustomCssWrapperConfig {

    private $idPrefix;
    
    
    public function __construct(\SimpleXMLElement $config) {
        $this->idPrefix = (string)$config->CustomJavascriptWrapper->IdPrefix;
    }
    
    /**
     * Each javscript file will have an associated string handle in Wordpress
     * that needs to be unique. This function returns the prefix for the handle.
     * The remaining part will be 
     */
    public function idPrefix() {
        return $this->idPrefix;
    }
}
