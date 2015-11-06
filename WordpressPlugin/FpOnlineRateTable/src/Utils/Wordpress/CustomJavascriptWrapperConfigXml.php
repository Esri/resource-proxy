<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once 'ICustomJavascriptWrapperConfig.php';


/**
 * Description of CustomJavascriptWrapperConfigXml
 *
 * @author scharfenberg
 */
class CustomJavascriptWrapperConfigXml
        implements ICustomJavascriptWrapperConfig {

    private $idPrefix;
    private $localizationObject;
    private $metaObject;
    
    
    public function __construct(\SimpleXMLElement $config) {
    
        $this->idPrefix = (string)$config->CustomJavascriptWrapper->IdPrefix;
        $this->localizationObject
                = (string)$config->CustomJavascriptWrapper->LocalizationObject;
        $this->metaObject
                = (string)$config->CustomJavascriptWrapper->MetaObject;
    }
    
    /**
     * Each javscript file will have an associated string handle in Wordpress
     * that needs to be unique. This function returns the prefix for the handle.
     * The remaining part will be 
     */
    public function idPrefix() {
        return $this->idPrefix;
    }
    
    /**
     * Wordpress will create a global Javascript object containing the
     * localization strings for scripts. This method returns the name of the
     * object.
     */
    public function localizationObject() {
        return $this->localizationObject;
    }
    
    /**
     * Wordpress will create a global Javascript object for passing additional
     * data to scripts. This method returns the name of the object.
     */
    public function metaObject() {
        return $this->metaObject;
    }
}
