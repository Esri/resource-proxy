<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once 'Helper.php';
require_once dirname(__DIR__) . '/Helper/ILogger.php';
require_once 'WordpressLogger.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\WordpressLogger;


/**
 * Description of LocalizationProvider
 *
 * @author scharfenberg
 */
class LocalizationTextDomain {
    
    private $languageDir;
    private $translationDomain;
    private $logger;
    
    
    public function load() {
        
        $result = load_plugin_textdomain(
            $this->translationDomain,
            false,
            $this->languageDir
        );
        if(false === $result) {
            $this->logger->addWarning(
                    "Localization file could not be loaded",
                    [   'textDomain' => $this->translationDomain,
                        'languageDir' => $this->languageDir]);
        }
    }
        
    public function loadOnAction($action = 'init') {
        add_action($action, [$this, 'load']);
    }
    
    // Note $languageDir has to be specified as a relative path with
    // respect to the Wordpress plugin directory (e.g. 'TDCInfo/Languages')
    public function __construct($languageDir, $domain,
            ILogger $logger = null) {
    
        $this->languageDir = $languageDir;
        
        // Note: Wordpress looks for translation files in the language
        // directory that look like "<translationDomain>-de_DE.mo".
        $this->translationDomain = $domain; 
        
        if(isset($logger)) {
            $this->logger = $logger;
        } else {
            $this->logger = WordpressLogger::create();
        }
    }
}
