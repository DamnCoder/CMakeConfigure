#include <stdio.h>

#include <folder_a/class_a.h>
#include <folder_b/class_b.h>
#include <folder_aaa/class_aaa.h>
#include <folder_aa/class_aa.h>

int main( int argc, char* args[] )
{
	dc::CClassAA* caa = new dc::CClassAA();
	dc::CClassA a;
	dc::CClassAA aa;
	dc::CClassAAA aaa;
	dc::CClassB b;
	printf("Hello World!\n");

    return 0;
}
