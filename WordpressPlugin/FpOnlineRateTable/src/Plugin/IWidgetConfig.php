<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin;

/**
 *
 * @author scharfenberg
 */
interface IWidgetConfig {
    
    /**
     * This is the Wordpress internal Id of the widget
     */
    public function id();
    
    /**
     * This is the html class name used for the widget container
     */
    public function htmlClass();
    
    /**
     * This is the shortcode used to place the widget on a page
     */
    public function shortcode();
    
    /**
     * This field is used to derive html id and html name of the widget settings
     * for the service Url.
     */
    public function serviceUrlId();
}
