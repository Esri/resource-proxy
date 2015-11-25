<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

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
     * The path relative to the plugin directory of the OnlineRateTable service
     * proxy.
     * Note: Currently the resource-proxy is in use that requires the
     * original service URL to be passed as parameter. This URL is meant to be
     * specified without this parameter.
     */
    public function serviceProxyPath();
}
