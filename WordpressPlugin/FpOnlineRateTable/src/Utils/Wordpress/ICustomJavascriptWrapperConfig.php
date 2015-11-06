<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

/**
 *
 * @author scharfenberg
 */
interface ICustomJavascriptWrapperConfig {
    
    /**
     * Each javscript file will have an associated string handle in Wordpress
     * that needs to be unique. This function returns the prefix for the handle.
     * The remaining part will be 
     */
    public function idPrefix();
    
    /**
     * Wordpress will create a global Javascript object containing the
     * localization strings for scripts. This method returns the name of the
     * object.
     */
    public function localizationObject();
    
    /**
     * Wordpress will create a global Javascript object for passing additional
     * data to scripts. This method returns the name of the object.
     */
    public function metaObject();
}
