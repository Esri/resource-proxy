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
interface ICustomCssWrapperConfig {
    
    /**
     * Each javscript file will have an associated string handle in Wordpress
     * that needs to be unique. This function returns the prefix for the handle.
     * The remaining part will be 
     */
    public function idPrefix();
}
